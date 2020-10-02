import { EventEmitter } from 'events'
import * as fs from 'fs'

import StrictEventEmitter from 'strict-event-emitter-types'
import WebSocket from 'ws'

import * as u from '../lib/util'
import * as afs from '../lib/atomicFS'
import deepClone from '../lib/deepClone'


export type GameEvent = {
  name: string
  data: object
}


export interface GameData {
  id: number
  userId1: number
  userId2: number
  events: GameEvent[]
}

export default class Game {
  // public data: GameData

  public code: string
  public player1?: WebSocket
  public player2?: WebSocket
  /** All connected players are viewers. */
  public viewers: WebSocket[] = [];

  /**
   * Game
   * @param data 
   */
  constructor(code: string) {
    this.code = code
  }
}
