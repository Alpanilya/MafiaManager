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
    return item?.ID ?? item?.id
  }

  function getName(item, defaultName = null) {
    return item?.Name ?? item?.name ?? defaultName
  }

  return {
    removeById,
    getLastID,
    getId,
    getName,
  }
}
