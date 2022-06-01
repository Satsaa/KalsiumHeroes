
export function parse(data: string): Command {
  return JSON.parse(data)
}

type Implements<T, U extends T> = {}

// List all events
export type Command =
  GameEvent | GameEventList | GameCreate | GenerateCode | GenerateCodeResult | GameDelete |
  GameDisconnect | GameJoin | GameLeave | RequestEvents | GameSpectate | Result

export interface GameEvent {
  command: 'GameEvent'
  data: {
    guid: string
    type: string
    code: string
    gameEventNum: number
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

export interface GenerateCode {
  command: 'GenerateCode'
  data: {
    guid: string
    type: 'GenerateCode'
  }
}

export interface GenerateCodeResult {
  command: 'GenerateCodeResult'
  data: {
    result: ResultType
    to: string
    type: 'GenerateCodeResult'
    code: string
  }
}

export interface GameDelete {
  command: 'GameDelete'
  data: {
    guid: string
    type: 'GameDelete'
    code: string
  }
}

export interface GameJoin {
  command: 'GameJoin'
  data: {
    guid: string
    type: 'GameJoin'
    code: string
    team: string
  }
}

export interface GameLeave {
  command: 'GameLeave'
  data: {
    guid: string
    type: 'GameLeave'
    code: string
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

export interface RequestEvents {
  command: 'RequestEvents'
  data: {
    guid: string
    type: 'RequestEvents'
    code: string
  }
}


export interface GameEventList {
  command: 'GameEventList'
  data: {
    type: string
    code: string
    types: string[]
    jsons: string[]
  }
}

export interface GameDisconnect {
  command: 'GameDisconnect'
  data: {
    type: 'GameDisconnect'
    code: string
    message?: string
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
