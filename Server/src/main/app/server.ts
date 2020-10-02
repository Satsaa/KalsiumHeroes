import { EventEmitter } from 'events'
import * as fs from 'fs'
import * as http from 'http'

import StrictEventEmitter from 'strict-event-emitter-types'
import WebSocket from 'ws'

import * as u from '../lib/util'
import * as afs from '../lib/atomicFS'
import deepClone from '../lib/deepClone'

import * as cmds from './cmds'
import Game from './game'

export interface Events {
  raw: (json: any) => void
}

export interface ServerOptions {
  /** Client data will be loaded from and saved in this directory's "global" folder */
  dataRoot: string
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

  private games: {[id: number]: Game} = {}
  private activeGames: {[code: string]: Game} = {}

  /**
   * Server
   * @param options 
   */
  constructor(options: ServerOptions) {
    this.opts = {
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

    const wss = new WebSocket.Server({ port: 8080 })
    wss.on('connection', this.onConnection)
  }

  private sendCmd(ws: WebSocket, cmd: cmds.Command) {
    ws.send(cmd)
  }
  private gameNotFound(ws: WebSocket) {
    this.sendCmd(ws, { command: 'game_missing', data: { message: 'Game not found.' } })
  }

  private createGame(): Game {
    const id = this.makeid()
    const game = new Game(id)
    this.activeGames[id] = game
    return game
  }

  private makeid(): string {
    let result = ''
    const characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789'
    do {
      for (let i = 0; i < 5; i++) {
        result += characters.charAt(Math.floor(Math.random() * characters.length))
      }
    } while (!this.activeGames[result])
    return result
  }

  private onConnection(ws: WebSocket, req: http.IncomingMessage): void {
    ws.on('message', this.onMessage)
  }

  private onMessage(ws: WebSocket, message: string): void {
    try {
      const cmd = cmds.parse(message)
      switch (cmd.command) {
        case 'game_create': {
          const game = this.createGame()
          this.sendCmd(ws, { command: 'game_created', data: { code: game.code } })
          break
        }

        case 'game_event': {
          const game = this.activeGames[cmd.data.code]
          if (!game) return this.gameNotFound(ws)

          // Event is sent to all connected viewers which includes the players
          for (const viewer of game.viewers) {
            if (viewer.readyState !== WebSocket.OPEN) continue
            this.sendCmd(ws, cmd)
          }
          break
        }

        case 'game_connect': {
          const game = this.activeGames[cmd.data.code]
          if (!game) return this.gameNotFound(ws)
          if (cmd.data.player) {
            if (cmd.data.player === 1) game.player1 = ws
            if (cmd.data.player === 2) game.player2 = ws
          } else if (!game.viewers.includes(ws)) {
            game.viewers.push(ws)
          }
          break
        }

        default: {
          this.sendCmd(ws, { command: 'unknown', data: { message: `Unknown command: "${cmd.command}"` } })
          break
        }
      }
    } catch (error) {
      this.sendCmd(ws, { command: 'invalid', data: { message: 'Malformed message' } })
    }
  }
}
