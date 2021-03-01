
export function parse(data: string): Command {
  return JSON.parse(data) as Command
}

type Implements<T, U extends T> = {}

// List all events
export type Command = GameEvent | GameCreate | GameConnect | Success | Fail | Error

export interface GameEvent {
  command: 'GameEvent'
  data: {
    type: string
    code: string
  }
}

export interface GameCreate {
  command: 'GameCreate'
  data: {
    type: 'GameCreate'
    code: string
  }
}

export interface GameConnect {
  command: 'GameConnect'
  data: {
    type: 'GameConnect'
    code: string
    player: -1 | 0 | 1
  }
}

export interface Success {
  command: 'Success'
  data: {
    type: 'Success'
    for: Command['command']
  }
}

export interface Fail {
  command: 'Fail'
  data: {
    type: 'Fail'
    for: Command['command']
    message?: string
  }
}

export interface Error {
  command: 'Error'
  data: {
    type: 'Error'
    message?: string
  }
}
