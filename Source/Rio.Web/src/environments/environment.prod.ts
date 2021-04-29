import { DynamicEnvironment } from './dynamic-environment';
class Environment extends DynamicEnvironment {

  constructor() {
    super(true);
  }
}

export const environment = new Environment();