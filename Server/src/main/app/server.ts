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

  private games: {[code: string]: Game} = {}

  /**
   * Server
   * @param options 
   */
  constructor(options: ServerOptions) {
    this.opts = {
      port: 8080,
      local: true,
      pingInterval: 30000,
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

    if (server) {
      const wss = new WebSocket.Server({ port: this.opts.port, server })
      wss.on('connection', this.onConnection.bind(this))
    } else {
      const wss = new WebSocket.Server({ port: this.opts.port })
      wss.on('connection', this.onConnection.bind(this))
    }
  }

  private onConnection(this: Server, ws: WebSocket, req: http.IncomingMessage): void {
    ws.on('message', this.onMessage.bind(this, ws))
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
    if (typeof message !== 'string') {
      this.sendCmd(ws, { command: 'Result', data: { type: 'Result', result: ResultType.Error, to: '', message: 'Message must be a string' } })
      return
    }
    try {
      const cmd = commands.parse(message)
      try {
        console.log(cmd)
        switch (cmd.command) {
          case 'GameCreate': {
            if (this.createGame(cmd.data.code)) {
              return this.sendCmd(ws, { command: 'Result', data: { type: 'Result', result: ResultType.Success, to: cmd.data.guid } })
            } else {
              return this.alreadyUsed(ws, cmd)
            }
          }

          case 'GameEvent': {
            const game = this.games[cmd.data.code]
            if (!game) return this.gameNotFound(ws, cmd)

            // Event is sent to all connected viewers which includes the players
            for (const viewer of game.viewers) {
              if (viewer.readyState !== WebSocket.OPEN) continue
              this.sendCmd(viewer, cmd)
            }
            break
          }

          case 'GameJoin': {
            const game = this.games[cmd.data.code]
            if (!game) return this.gameNotFound(ws, cmd)

            // Other live ws occupying team
            const teamWs = game.players[cmd.data.team]
            if (teamWs && teamWs !== ws && teamWs.readyState === WebSocket.OPEN) {
              return this.sendCmd(ws, { command: 'Result', data: { type: 'Result', result: ResultType.Fail, to: cmd.data.guid, message: 'Team occupied' } })
            }

            game.players[cmd.data.team] = ws
            if (!game.viewers.includes(ws)) game.viewers.push(ws)

            return this.success(ws, cmd)
            break
          }

          case 'GameSpectate': {
            const game = this.games[cmd.data.code]
            if (!game) return this.gameNotFound(ws, cmd)
            if (!game.viewers.includes(ws)) game.viewers.push(ws)
            return this.success(ws, cmd)
          }

          default: {
            return this.sendCmd(ws, { command: 'Result', data: { type: 'Result', result: ResultType.Error, to: (cmd as any).guid ?? '', message: `Unknown or unexpected command: "${cmd.command}"` } })
          }
        }
      } catch (error) {
        this.sendCmd(ws, { command: 'Result', data: { type: 'Result', result: ResultType.Error, to: (cmd as any).guid ?? '', message: 'Message caused a server error' } })
      }
    } catch (error) {
      this.sendCmd(ws, { command: 'Result', data: { type: 'Result', result: ResultType.Error, to: '', message: 'Malformed message' } })
    }
  }


  private success(ws: WebSocket, cmd: Command & {data: { guid: string }}) {
    this.sendCmd(ws, { command: 'Result', data: { type: 'Result', result: ResultType.Success, to: cmd.data.guid } })
  }
  private alreadyUsed(ws: WebSocket, cmd: Command & {data: { guid: string }}) {
    this.sendCmd(ws, { command: 'Result', data: { type: 'Result', result: ResultType.Fail, to: cmd.data.guid, message: 'Code already used' } })
  }

  private gameNotFound(ws: WebSocket, cmd: Command & {data: { guid: string }}) {
    this.sendCmd(ws, { command: 'Result', data: { type: 'Result', result: ResultType.Fail, to: cmd.data.guid, message: 'Game not found' } })
  }
}
