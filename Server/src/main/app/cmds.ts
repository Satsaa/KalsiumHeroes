
export function parse(data: string): Cmd {
  const space = data.indexOf(' ')
  if (space === -1) throw new Error('Invalid data')

  const cmd = data.substr(0, space)
  const cmdData = JSON.parse(data.substr(space + 1))

  return {
    cmd,
    cmdData,
  } as unknown as Cmd
}

export type Cmd = GameEvent | GameCreate | GameCreated | GameMissing | GameConnect | Unknown | Invalid

export interface GameEvent {
  cmd: 'game_event'
  data: {
    code: string
    eventNum: number
    name: string
    data: object
  }
}

export interface GameCreate {
  cmd: 'game_create'
  data: {}
}

export interface GameCreated {
  cmd: 'game_created'
  data: {
    code: string
  }
}

export interface GameMissing {
  cmd: 'game_missing'
  data: {
    message?: string
  }
}

export interface GameConnect {
  cmd: 'game_connect'
  data: {
    code: string
  }
}

export interface Unknown {
  cmd: 'unknown'
  data: {
    message?: string
  }
}

export interface Invalid {
  cmd: 'invalid'
  data: {
    message?: string
  }
}
