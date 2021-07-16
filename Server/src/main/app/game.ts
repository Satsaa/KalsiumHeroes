import { EventEmitter } from 'events'
import * as fs from 'fs'

import StrictEventEmitter from 'strict-event-emitter-types'
import WebSocket from 'ws'

import * as u from '../lib/util'
import * as afs from '../lib/atomicFS'
import deepClone from '../lib/deepClone'

import { GameEvent, Team } from './commands'

export default class Game {
  // public data: GameData

  public code: string
  public events: GameEvent[] = []
  public players: {[team: number]: WebSocket} = Object.create(null);
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
