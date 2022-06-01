
import WebSocket from 'ws'

import Game from './game'

export class Connection {
  public ws!: WebSocket
  public viewing: code[] = []
  public playedGames: Array<[code, team]> = []
  public lastActivity: number = Date.now()

  constructor(ws: WebSocket) {
    this.ws = ws
  }
}
