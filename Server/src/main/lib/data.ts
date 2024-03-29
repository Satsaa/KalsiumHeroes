import { EventEmitter } from 'events'
import * as fs from 'fs'

import logger from '../logger'

import * as afs from './atomicFS'
import defaultKeys from './defaultKeys'

interface LoadOptions<T> {
  /** If the file doesn't exist, create it with this data */
  defaultData?: T
  /** Sets all undefined keys in the returned data that exist in `defaultData` to the corresponding value in `defaultData` */
  defineUndefined?: boolean
  /** Disable saving of the data */
  noSave?: boolean
}

export default class Data extends EventEmitter {
  /** This data changes when events happen */
  public data: { [group: string]: { [name: string]: { [x: string]: any } } }

  public dataPath: string
  /** Reserved data names. No data can be loaded or autoloaded with one of these names */
  private reserved: readonly string[]
  /** Loading data is blocked and throws */
  private blockLoads: { [group: string]: { [name: string]: true } }
  /** Saving data is blocked and throws */
  private blockSaves: { [group: string]: { [name: string]: true } }
  private autoLoads: Array<{ name: string, defaultData?: object, setDefaults?: boolean }>

  constructor(dataRoot: string, reserved = ['']) {
    super()
    this.data = {}

    this.dataPath = dataRoot
    if (!fs.existsSync(dataRoot)) fs.mkdirSync(dataRoot)

    this.reserved = reserved

    this.blockLoads = {}
    this.blockSaves = {}
    this.autoLoads = []
  }

  /** Returns the path to the data file */
  public getPath(group: string | number, name: string, fileType: string = 'json') {
    return `${this.dataPath}/${group}/${name}.${fileType}`
  }

  /**
   * Returns the data or undefined if it isn't loaded.  
   * Data will be an object and therefore a reference, so keep that it mind. The undefined value is not a reference
   */
  public getData<T>(group: string | number, name: string): T | undefined {
    if ((this.data[group] || {})[name]) return this.data[group][name] as T
    return undefined
  }
  /** Sets the data variable to `value` */
  public setData<T extends { [x: string]: any }>(group: string | number, name: string, value: T) {
    if (!this.data[group]) this.data[group] = {}
    this.data[group][name] = value
    return value
  }

  /** Wait until the data is loaded. Resolves with the data or undefined if timedout */
  public async waitData<T>(group: string | number, name: string, timeout?: number): Promise<T | undefined> {
    if (this.getData(group, name)) return this.getData(group, name)
    return new Promise((resolve) => {
      const cbFunc = (s?: string, n?: string, data?: T) => {
        if (s && n) if (s !== group || n !== name) return
        this.removeListener('load', cbFunc)
        clearTimeout(_timeout)
        resolve(data)
      }
      let _timeout: number
      if (timeout !== undefined) _timeout = setTimeout(cbFunc, timeout)
      this.on('load', cbFunc)
    })
  }

  public saveAllSync() {
    for (const group in this.data) {
      for (const name in this.data[group]) {
        const data = this.getData(group, name)
        if (typeof data === 'object') {
          try {
            if (!this.blockSaves[group][name]) {
              const path = `${this.dataPath}/${group}/${name}.json`
              const tempPath = `${this.dataPath}/${group}/${name}_temp.json`
              fs.writeFileSync(tempPath, JSON.stringify(data, null, 0))
              fs.renameSync(tempPath, path)
            }
          } catch (err) {
            logger.error(`Failed to save ${group}\\${name}:`)
            logger.error(err)
          }
        } else {
          logger.error(new Error(`Failed to save ${group}\\${name} because it's type was ${typeof data}`))
        }
      }
    }
  }

  /**
   * Saves a file in `Data.dataPath`/`group`/`name`
   * @param group E.g. 'default', 'global'
   * @param name File name
   * @param unload Unload from memory if save is succesful
   */
  public async save(group: string | number, name: string, unload: boolean = false) {
    const data = this.getData(group, name)
    if (typeof data !== 'object') {
      logger.warn(new Error(`Failed to save ${group}\\${name} because it's type was ${typeof data}`))
      return false
    }
    try {
      if (this.blockSaves[group][name]) throw new Error('Saving is blocked for this data type')
      await afs.writeFile(`${this.dataPath}/${group}/${name}.json`, JSON.stringify(data, null, 0))
      if (unload) this.delData(group, name)
      return true
    } catch (err) {
      logger.warn(`Could not save ${group}\\${name}:`, err)
      return false
    }
  }

  /**
   * Reloads a file in `Data.dataPath`/`group`/`name`
   * @param group E.g. 'default', 'global'.
   * @param name File name
   * @param save Save before reloading
   */
  public async reload<T>(group: string | number, name: string, save: boolean = false) {
    if (save) await this.save(group, name)
    if (!this.getData(group, name)) throw new Error(`${group}\\${name} cannot be reloaded as it is not loaded`)
    this.delData(group, name)
    return this.load<T>(group, name)
  }

  /**
   * Loads a file in `Data.dataPath`/`group`/`name`
   * @param name File name
   * @param opts Options
   */
  public async load<T>(group: string | number, name: string, opts: LoadOptions<T> = {}): Promise<T> {
    if (!this.data[group]) this.data[group] = {}
    if (String(group).length === 0 || name.length === 0) throw new Error('group and name must not be zero-length')
    if (this.reserved.includes(name)) throw new Error(`${name} is reserved for internal functions`)
    if (this.getData(group, name)) throw new Error(`${name} has already been loaded by another source`)
    if (opts.noSave) this.blockSave(group, name)
    this.blockLoad(group, name)

    const file = `${this.dataPath}/${group}/${name}.json`
    try { // Check if file is already created
      await afs.access(file, fs.constants.F_OK)
    } catch (err) {
      if (err.code !== 'ENOENT') throw err
      if (opts.defaultData) {
        const pathOnly = file.slice(0, file.lastIndexOf('/'))
        try { // Ensure directory exists
          await afs.access(file, fs.constants.F_OK)
        } catch (err) {
          if (err.code !== 'ENOENT') throw err
          await afs.mkdir(pathOnly, { recursive: true })
        }

        const result = this.setData(group, name, opts.defaultData)
        this.emit('load', group, name, result)
        this.save(group, name).catch((err) => { throw err })
        return result
      } else {
        throw new Error('Cannot load file that doesn\'t exist. Define defaultData if you want to create it if needed')
      }
    }

    let data
    try {
      data = JSON.parse(await afs.readFile(file, 'utf8'))
    } catch (err) {
      throw new Error(`${file} is corrupted: ${err.name as string}`)
    }
    if (typeof data !== 'object') throw new Error(`Wrong data type in file: ${typeof data}`)
    if (opts.defineUndefined) defaultKeys(data, opts.defaultData || {})
    if (typeof data !== 'object') throw new Error(`Data became corrupted: ${typeof data}`)

    const result = this.setData(group, name, data)
    this.emit('load', group, name, result)
    return result
  }

  /** Delete the data */
  private delData(group: string | number, name: string) {
    this.blockLoad(group, name, true)
    this.blockSave(group, name, true)
    if (this.data[group]) delete this.data[group][name]
  }

  /** Blocks or unblocks the loading of a data type. Attempting to load a blocked data type will throw as a duplicate */
  private blockLoad(group: string | number, name: string, unblock = false) {
    if (unblock) {
      if (!this.blockLoads[group]) return
      delete this.blockLoads[group][name]
    } else {
      if (!this.blockLoads[group]) this.blockLoads[group] = {}
      this.blockLoads[group][name] = true
    }
  }

  /** Blocks or unblocks the saving of a data type. Attempting to save a blocked data type will throw */
  private blockSave(group: string | number, name: string, unblock = false) {
    if (unblock) {
      if (!this.blockSaves[group]) return
      delete this.blockSaves[group][name]
    } else {
      if (!this.blockSaves[group]) this.blockSaves[group] = {}
      this.blockSaves[group][name] = true
    }
  }
}
