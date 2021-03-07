
export function parse(data: string): Command {
  return JSON.parse(data)
}

type Implements<T, U extends T> = {}

// List all events
export type Command = GameEvent | GameCreate | GameJoin | GameSpectate | Result

export interface GameEvent {
  command: 'GameEvent'
  data: {
    guid: string
    type: string
    code: string
  }
}

export interface GameCreate {
  command: 'GameCreate'
  data: {
    guid: string
    type: 'GameCreate'
    code: string
  }
}

export interface GameJoin {
  command: 'GameJoin'
  data: {
    guid: string
    type: 'GameJoin'
    code: string
    team: number
  }
}

export interface GameSpectate {
  command: 'GameSpectate'
  data: {
    guid: string
    type: 'GameSpectate'
    code: string
  }
}

export interface Result {
  command: 'Result'
  data: {
    type: 'Result'
    result: ResultType
    to: string
    message?: string
  }
}

export enum ResultType {
  Timeout = -1,
  Success,
  Fail,
  Error,
}

export enum Team {
  Team1,
  Team2,
}
