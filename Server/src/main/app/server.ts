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

  private onConnection(ws: WebSocket, req: http.IncomingMessage) {
    ws.on('message', this.onMessage)
  }

  private onMessage(ws: WebSocket, message: string) {
    try {
      const cmd = cmds.parse(message)
      switch (cmd.cmd) {
        case 'game_create': {
          const game = this.createGame()
          const data: cmds.GameCreated = { cmd: 'game_created', data: { code: game.code } }
          ws.send(data)
          break
        }

        case 'game_event': {
          const game = this.activeGames[cmd.data.code]
          if (!game) {
            const data: cmds.GameMissing = { cmd: 'game_missing', data: { message: 'Game not found.' } }
            ws.send(data)
            break
          }

          // Event is sent to all connected viewers which includes the players
          for (const viewer of game.viewers) {
            if (viewer.readyState !== WebSocket.OPEN) continue
            viewer.send(cmd)
          }
          break
        }

        default: {
          const data: cmds.Unknown = { cmd: 'unknown', data: { message: `Unknown command: "${cmd.cmd}"` } }
          ws.send(data)
          break
        }
      }
    } catch (error) {
      const data: cmds.Invalid = { cmd: 'invalid', data: { message: 'Malformed message' } }
      ws.send(data)
    }
  }
}
