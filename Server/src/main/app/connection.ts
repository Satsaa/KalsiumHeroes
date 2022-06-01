
import WebSocket from 'ws'

export class Connection {
  public ws!: WebSocket
  public spectates: code[] = []
  public playedGames: Array<[code, team]> = []
  public lastActivity: number = Date.now()

  constructor(ws: WebSocket) {
    this.ws = ws
  }
}
