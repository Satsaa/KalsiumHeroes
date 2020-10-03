
export function parse(data: string): Command {
  const space = data.indexOf(' ')
  if (space === -1) throw new Error('Invalid data')

  const cmd = data.substr(0, space)
  const cmdData = JSON.parse(data.substr(space + 1))

  return {
    cmd,
    cmdData,
  } as unknown as Command
}

export type Command = GameEvent | GameCreate | GameCreated | GameMissing | GameConnect | Unknown | Invalid

export interface GameEvent {
  command: 'game_event'
  data: {
    code: string
    eventNum: number
    name: string
    data: object
  }
}

export interface GameCreate {
  command: 'game_create'
  data: {}
}

export interface GameCreated {
  command: 'game_created'
  data: {
    code: string
  }
}

export interface GameMissing {
  command: 'game_missing'
  data: {
    message?: string
  }
}

export interface GameConnect {
  command: 'game_connect'
  data: {
    code: string
    player?: 1 | 2
  }
}

export interface Unknown {
  command: 'unknown'
  data: {
    message?: string
  }
}

export interface Invalid {
  command: 'invalid'
  data: {
    message?: string
  }
}
