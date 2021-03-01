import deepClone from '../lib/deepClone'
import { onExit } from '../lib/util'
import { getArgs } from '../argRules'
import logger, { options as logOpts } from '../logger'
import Data from '../lib/data'
import * as secretKey from '../lib/secretKey'

import Server, { ServerOptions } from './server'

export interface AppOptions {
}

export default class App {
  private server: Server
  private data: Data
  private opts: Required<AppOptions>
  private args: ReturnType<typeof getArgs>

  constructor(options: AppOptions) {
    this.opts = {
      ...deepClone(options),
    }

    onExit(this.onExit.bind(this))

    this.args = getArgs()
    if (Array.isArray(this.args)) throw this.args

    if (this.args.args['preserve-log']) logOpts.noSave = true

    if (this.args.args.global) {
      const _global = global as any
      if (_global[this.args.args.global[0] || 'app']) {
        throw new Error(`global[${this.args.args.global[0] || 'app'}] is already defined, define a different value for --global`)
      } else {
        _global[this.args.args.global[0] || 'app'] = this
      }
    }

    const configPath = './cfg/keys.json'

    const port = secretKey.getKey(configPath, 'server', 'port')
    const local = secretKey.getKey(configPath, 'server', 'local') !== undefined

    this.server = new Server({
      dataRoot: './data/',
      port: typeof port === 'string' ? parseInt(port) : undefined,
      local,
    })

    this.data = new Data('./data/', ['games', 'users'])
  }

  private onExit(code: number) {
    if (this.data) this.data.saveAllSync()
  }
}
