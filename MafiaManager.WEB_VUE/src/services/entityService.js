export function entityService() {
  function removeById(array, id) {
    if (!array || !id) {
      return
    }

    if (!Array.isArray(array)) {
      return
    }

    const index = array.findIndex((item) => getId(item) === id)

    if (index !== -1) {
      array.splice(index, 1)
    }
  }

  function getLastID(array) {
    if (!array) {
      return 0
    }

    if (!Array.isArray(array)) {
      return -1
    }

    if (array.length === 0) {
      return 0
    }

    return Math.max(...array.map((i) => getId(i)))
  }

  function getId(item) {
    return getProperty(item, 'ID', 'id')
  }

  function getName(item, defaultName = null) {
    return getProperty(item, 'Name', 'name') ?? defaultName
  }

  function formatDate(date, format) {
    if (!date) return

    const defaultFormat = {
      hour: '2-digit',
      minute: '2-digit',
      second: '2-digit',
    }

    const actualFormat = { ...defaultFormat, ...format }

    return new Date(date).toLocaleTimeString('ru-RU', actualFormat)
  }

  function getPropertyName(object, ...propertyNames) {
    if (!object) return

    return propertyNames.find((propertyName) => propertyName in object)
  }

  function getProperty(object, ...propertyNames) {
    if (!object) return

    const propertyName = getPropertyName(object, ...propertyNames)

    return object[propertyName]
  }

  return {
    removeById,
    getLastID,
    getId,
    getName,
    formatDate,
    getPropertyName,
    getProperty,
  }
}
