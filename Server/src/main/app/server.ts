import { EventEmitter } from 'events'
import * as fs from 'fs'
import * as http from 'http'

import StrictEventEmitter from 'strict-event-emitter-types'
import WebSocket from 'ws'

import * as u from '../lib/util'
import * as afs from '../lib/atomicFS'
import deepClone from '../lib/deepClone'

import * as commands from './commands'
import { Command, ResultType } from './commands'
import Game from './game'
import { Connection } from './connection'

export interface Events {
  raw: (json: any) => void
}

export interface ServerOptions {
  /** Client data will be loaded from and saved in this directory's "global" folder */
  dataRoot: string
  port?: number
  local?: boolean
  pingInterval?: number
}

export default class Server {
  public opts: Required<ServerOptions>

  public on: StrictEventEmitter<EventEmitter, Events>['on']
  public once: StrictEventEmitter<EventEmitter, Events>['once']
  public prependListener: StrictEventEmitter<EventEmitter, Events>['prependListener']
  public prependOnceListener: StrictEventEmitter<EventEmitter, Events>['prependOnceListener']
  public removeListener: StrictEventEmitter<EventEmitter, Events>['removeListener']
  public emit: StrictEventEmitter<EventEmitter, Events>['emit']

  private games: {[code: code]: Game} = {}
  private connections: Map<WebSocket, Connection> = new Map<WebSocket, Connection>()

  /**
   * Server
   * @param options 
   */
  constructor(options: ServerOptions) {
    this.opts = {
      port: 8080,
      local: true,
      pingInterval: 10000,
      ...deepClone(options),
    }

    // Do this isntead of using "extends" so event typings work
    const emitter = new EventEmitter()
    this.on = emitter.on
    this.once = emitter.once
    this.prependListener = emitter.prependListener
    this.prependOnceListener = emitter.prependOnceListener
    this.removeListener = emitter.removeListener
    this.emit = emitter.emit

    const server = this.opts.local ? undefined : new http.Server()

    const wss = server ?
      new WebSocket.Server({ port: this.opts.port, server }) :
      new WebSocket.Server({ port: this.opts.port })
    const addr = wss.address() as WebSocket.AddressInfo

    wss.on('connection', this.onConnection.bind(this))

    const now = Date.now()

    const interval = setInterval(() => {
      for (const connection of this.connections.values()) {
        if (connection.lastActivity < now - this.opts.pingInterval) return connection.ws.terminate()
        connection.ws.ping()
      }
    }, this.opts.pingInterval)

    wss.on('close', () => {
      clearInterval(interval)
    })

    console.log(`Hosting: ${addr ? `${addr.family} ${addr.address} ${addr.port}` : 'Local'}`)
  }

  public codeGen(length = 4): string {
    let code = ''
    for (let i = 0; i < length; i++) {
      let rng = Math.floor(Math.random() * 59)
      if (rng <= 9) { code += String.fromCharCode(48 + rng) } else {
        rng -= 9
        if (rng <= 25) { code += String.fromCharCode(65 + rng) } else {
          rng -= 25
          code += String.fromCharCode(97 + rng)
        }
      }
    }
    if (this.games[code]) return this.codeGen(length + 1)
    return code
  }

  private onConnection(this: Server, ws: WebSocket, req: http.IncomingMessage): void {
    ws.on('message', this.onMessage.bind(this, ws))
    ws.on('pong', this.onPong.bind(this, ws))
    ws.on('close', this.onClose.bind(this, ws))

    const connection = new Connection(ws)
    this.connections.set(ws, connection)
  }
  private onPong(ws: WebSocket) {
    const connection = this.connections.get(ws)
    if (!connection) return ws.terminate()
    connection.lastActivity = Date.now()
  }
  private onClose(ws: WebSocket) {
    const connection = this.connections.get(ws)
    this.connections.delete(ws)
    if (connection) {
      // Remove spectator
      for (const spectateCode of connection.viewing) {
        const spectate = this.games[spectateCode]
        if (spectate) {
          spectate.viewers.filter(v => v === ws)
        }
      }
      // Remove player
      for (const playedGameCodeAndTeam of connection.playedGames) {
        const playedGameCode = playedGameCodeAndTeam[0]
        const playedGameTeam = playedGameCodeAndTeam[1]
        const playedGame = this.games[playedGameCode]
        if (playedGame) {
          const playerWs = playedGame.players[playedGameTeam]
          if (playerWs === ws) {
            delete playedGame.players[playedGameTeam]
          }
        }
      }
    }
  }

  private sendCmd(ws: WebSocket, cmd: commands.Command) {
    ws.send(JSON.stringify(cmd))
  }

  private createGame(code: string): boolean {
    if (this.games[code]) return false

    const game = new Game(code)
    this.games[code] = game
    return true
  }

  private onMessage(this: Server, ws: WebSocket, message: string): void {
    const connection = this.connections.get(ws)
    if (!connection) return ws.terminate()
    connection.lastActivity = Date.now()

    if (typeof message !== 'string') {
      this.sendCmd(ws, { command: 'Result', data: { type: 'Result', result: ResultType.Error, to: '', message: 'ServerInternal_MessageNotString' } })
      return
    }
    try {
      const cmd = commands.parse(message)
      try {
        console.log(cmd)
        switch (cmd.command) {
          case 'GameCreate': {
            if (this.createGame(cmd.data.code)) {
              return this.success(ws, cmd)
            } else {
              return this.alreadyUsed(ws, cmd)
            }
          }

          case 'GenerateCode': {
            const code = this.codeGen()
            if (this.createGame(code)) {
              return this.sendCmd(ws, { command: 'GenerateCodeResult', data: { type: 'GenerateCodeResult', result: ResultType.Success, to: cmd.data.guid, code } })
            } else {
              return this.sendCmd(ws, { command: 'GenerateCodeResult', data: { type: 'GenerateCodeResult', result: ResultType.Fail, to: cmd.data.guid, code } })
            }
          }

          case 'GameEvent': {
            const game = this.games[cmd.data.code]
            if (!game) return this.gameNotFound(ws, cmd)

            if (cmd.data.gameEventNum < game.events.length) {
              for (let i = Math.max(0, cmd.data.gameEventNum); i < game.events.length; i++) {
                this.sendCmd(ws, game.events[i])
              }
              return this.incorrectGameEventId(ws, cmd)
            } else if (cmd.data.gameEventNum > game.events.length) {
              return this.incorrectGameEventId(ws, cmd)
            }

            game.events.push(cmd)

            // Event is sent to all connected viewers which includes the players
            for (const viewer of game.viewers) {
              if (viewer.readyState !== WebSocket.OPEN) continue
              this.sendCmd(viewer, cmd)
            }
            return this.success(ws, cmd)
          }

          case 'GameJoin': {
            const game = this.games[cmd.data.code]
            if (!game) return this.gameNotFound(ws, cmd)

            // Other live ws occupying team?
            const playerWs = game.players[cmd.data.team]
            if (playerWs && playerWs !== ws && playerWs.readyState === WebSocket.OPEN) {
              return this.fail(ws, cmd, 'Server_SlotOccupied')
            }
            if (playerWs === ws) {
              return this.fail(ws, cmd, 'Server_AlreadyInSameGame')
            }
            if (connection.playedGames.length > 0) {
              return this.fail(ws, cmd, 'Server_AlreadyInOtherGame')
            }

            game.players[cmd.data.team] = ws
            connection.playedGames.push([cmd.data.code, cmd.data.team])
            if (!game.viewers.includes(ws)) game.viewers.push(ws)
            if (connection.viewing.includes(cmd.data.code)) connection.viewing.push(cmd.data.code)
            this.sendEvents(ws, game)

            return this.success(ws, cmd)
          }

          case 'GameSpectate': {
            const game = this.games[cmd.data.code]
            if (!game) return this.gameNotFound(ws, cmd)
            if (!game.viewers.includes(ws)) game.viewers.push(ws)
            if (connection.viewing.includes(cmd.data.code)) connection.viewing.push(cmd.data.code)
            this.sendEvents(ws, game)
            return this.success(ws, cmd)
          }

          case 'RequestEvents': {
            const game = this.games[cmd.data.code]
            if (!game) return this.gameNotFound(ws, cmd)
            this.success(ws, cmd)
            return this.sendEvents(ws, game)
          }

          case 'GameDelete': {
            const game = this.games[cmd.data.code]
            if (!game) return this.gameNotFound(ws, cmd)

            for (const viewer of game.viewers) {
              if (viewer.readyState !== WebSocket.OPEN || viewer === ws) continue
              this.sendCmd(viewer, {
                command: 'GameDisconnect',
                data: {
                  type: 'GameDisconnect',
                  code: game.code,
                  message: 'Server_DisconnectGameDeleted',
                },
              })
            }
            delete this.games[cmd.data.code]
            return this.success(ws, cmd)
          }

          case 'GameLeave': {
            const game = this.games[cmd.data.code]
            if (!game) return this.gameNotFound(ws, cmd)


            connection.playedGames.filter(v => v[0] === cmd.data.code)
            if (game.viewers.includes(ws)) game.viewers.filter(v => v === ws)
            connection.viewing.filter(v => v === cmd.data.code)

            return this.success(ws, cmd)
          }

          default: {
            return this.sendCmd(ws, { command: 'Result', data: { type: 'Result', result: ResultType.Error, to: (cmd as any).guid ?? '', message: 'ServerInternal_UnknownCommand' } })
          }
        }
      } catch (error) {
        this.sendCmd(ws, { command: 'Result', data: { type: 'Result', result: ResultType.Error, to: (cmd as any).guid ?? '', message: 'ServerInternal_InternalError' } })
      }
    } catch (error) {
      this.sendCmd(ws, { command: 'Result', data: { type: 'Result', result: ResultType.Error, to: '', message: 'ServerInternal_MalformedMessage' } })
    }
  }


  private success(ws: WebSocket, cmd: Command & {data: { guid: string }}) {
    this.sendCmd(ws, { command: 'Result', data: { type: 'Result', result: ResultType.Success, to: cmd.data.guid } })
  }
  private fail(ws: WebSocket, cmd: Command & {data: { guid: string }}, message: string) {
    this.sendCmd(ws, { command: 'Result', data: { type: 'Result', result: ResultType.Fail, to: cmd.data.guid, message } })
  }
  private alreadyUsed(ws: WebSocket, cmd: Command & {data: { guid: string }}) {
    this.sendCmd(ws, { command: 'Result', data: { type: 'Result', result: ResultType.Fail, to: cmd.data.guid, message: 'Server_CodeAlreadyInUse' } })
  }
  private incorrectGameEventId(ws: WebSocket, cmd: Command & {data: { guid: string }}) {
    this.sendCmd(ws, { command: 'Result', data: { type: 'Result', result: ResultType.Fail, to: cmd.data.guid, message: 'Server_GameEventIdDesynced' } })
  }
  private gameNotFound(ws: WebSocket, cmd: Command & {data: { guid: string }}) {
    this.sendCmd(ws, { command: 'Result', data: { type: 'Result', result: ResultType.Fail, to: cmd.data.guid, message: 'Server_GameNotFound' } })
  }
  private sendEvents(ws: WebSocket, game: Game) {
    const cmd: commands.GameEventList = {
      command: 'GameEventList',
      data: {
        type: 'GameEventList',
        code: game.code,
        types: game.events.map(v => v.data.type),
        jsons: game.events.map(v => JSON.stringify(v.data)),
      },
    }
    this.sendCmd(ws, cmd)
  }
}
