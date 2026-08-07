import { createApp, ref, watch, onMounted, computed } from 'https://unpkg.com/vue@3/dist/vue.esm-browser.prod.js'

function globalHandle() {
    const getLastID = (array) => {
        if (!array || array.length === 0)
            return 0;

        return Math.max(...array.map(i => getEntityId(i)));
    }

    function getEntityId(item) {
        return item?.ID ?? item?.id;
    }

    function getEntityName(item, defaultName = null) {
        return item?.Name ?? item?.name ?? defaultName;
    }

    function removeById(array, id) {
        const index = array.findIndex(item => getEntityId(item) === id);
        if (index !== -1) {
            array.splice(index, 1);
        }
    }

    return {
        getLastID,
        getEntityId,
        getEntityName,
        removeById,
    }
}

function rolesHandle(initRoles, defaultColor = '#FF0000') {
    const globalHandler = globalHandle();
    const roles = ref(initRoles);
    const newRoleName = ref('');
    const newRoleColor = ref(defaultColor);

    function getRoleColor(r) {
        return r?.Color ?? r?.color ?? defaultColor;
    }

    function appendCustomRole() {
        const name = newRoleName.value.trim();
        if (!name)
            return;

        const nextId = globalHandler.getLastID(roles.value) + 1;

        const role = {
            ID: nextId,
            Name: name,
            Color: newRoleColor.value
        };

        roles.value.push(role);

        newRoleName.value = '';
    }

    function setRoles(items) {
        roles.value = items;
    }

    return {
        roles,
        newRoleName,
        newRoleColor,
        getRoleColor,
        appendCustomRole,
        setRoles
    };
};

function playersHandle(initPlayers) {
    const globalHandler = globalHandle();
    const players = ref(initPlayers);
    const playersCount = ref(0);
    const defaultRole = ref(null);

    watch(playersCount, (newValue, oldValue) => {
        const diff = newValue - players.value.length;
        if (diff > 0) {
            appendPlayers(diff);
        }
        else {
            removePlayers(Math.abs(diff));
        }
    });

    function appendPlayers(count) {
        for (let i = 1; i <= count; i++) {
            appendPlayer();
        }
    }

    function appendPlayer() {
        const id = globalHandler.getLastID(players.value) + 1;

        const player = {
            "State": 0,
            "ID": id,
            "Name": `Игрок ${id}`,
            "Role": defaultRole.value
        }

        players.value.push(player);
    }

    function removePlayers(count) {
        for (let i = 1; i <= count; i++) {
            removeLastPlayer();
        }
    }

    function removeLastPlayer() {
        const id = globalHandler.getLastID(players.value);

        globalHandler.removeById(players.value, id);
    }

    function changePlayerRole(player, role) {
        player.Role = role;
    }

    function getPlayerRole(player) {
        return player?.Role ?? player?.role;
    }

    function setPlayers(items) {
        players.value = items;
        playersCount.value = items.length;
    }

    function getPlayersAlive(players) {
        return players.filter(i => (i.State ?? i.state) === 0)
    }

    function setDefaultRole(role) {
        defaultRole.value = role
    }

    return {
        players,
        playersCount,
        changePlayerRole,
        getPlayerRole,
        setPlayers,
        setDefaultRole
    }
};

function gameSettingsHandle() {
    const globalHandler = globalHandle();
    const rolesHandler = rolesHandle([]);
    const playerHandler = playersHandle();

    const settings = computed(() => ({
        ...baseSettings,
        roles: rolesHandler.roles.value,
        players: playerHandler.players.value,
        timeDelay: timeDelay.value,
    }));

    const timeDelay = ref(null);
    let modalInstance = null;
    let baseSettings = {};

    async function setDefaultSettings() {
        const currentSettings = await getGameSettings();

        baseSettings = currentSettings;

        const roles = currentSettings.Roles ?? currentSettings.roles;
        const players = currentSettings.Players ?? currentSettings.players
        const currentTimeDelay = currentSettings?.TimeDelay ?? currentSettings?.timeDelay;
        const defaultRole = currentSettings.DefaultRole ?? currentSettings.defaultRole;

        if (roles) {
            rolesHandler.setRoles(roles);
        }

        if (players) {
            playerHandler.setPlayers(players);
        }

        if (currentTimeDelay)
            timeDelay.value = currentTimeDelay


        if (defaultRole) {
            playerHandler.setDefaultRole(defaultRole);
        }

        const modalElement = document.getElementById('settingsModal');

        if (modalElement) {
            modalInstance = new bootstrap.Modal(modalElement);
            modalInstance.show();
        }
    };

    async function getGameSettings() {
        const response = await fetch('https://localhost:7001/api/game/getGameSettings', {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        return await response.json();
    }

    async function startGame() {
        if (timeDelay.value <= 0) {
            alert('Время на ходы должно быть больше 0');
            return;
        }

        if (playerHandler.playersCount?.value <= 0) {
            alert('Должен быть хотя бы один игрок');
            return;
        }

        if (rolesHandler.roles?.value.length === 0) {
            alert('Должна быть хотя бы одна роль');
            return;
        }

        try {
            const response = await fetch('https://localhost:7001/api/game/create', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(settings.value)
            });

            const gameInstance = await response.json();

            window.dispatchEvent(new CustomEvent('onGameStarted', {
                detail: gameInstance
            }));

            if (modalInstance) {
                modalInstance.hide();
            }

        } catch (error) {
            alert('Ошибка при запуске игры:', error);
        }
    }

    return {
        timeDelay,
        startGame,
        setDefaultSettings,
        ...globalHandler,
        ...rolesHandler,
        ...playerHandler
    };
}

export { globalHandle, rolesHandle, playersHandle, gameSettingsHandle }
