import { createApp, ref, watch, onMounted } from 'https://unpkg.com/vue@3/dist/vue.esm-browser.prod.js'

const settings = window.gameSettings;

function global() {
    const getLastID = (array) => {
        if (array.length === 0)
            return 0;

        return Math.max(...array.map(i => getId(i)));
    }

    const getId = (item) => item.ID;

    function removeById(array, id) {
        const index = array.findIndex(r => getId(r) === id);
        if (index !== -1) {
            array.splice(index, 1);
        }
    }

    return {
        getLastID,
        getId,
        removeById
    }
}

function rolesHandle(defaultColor = '#FF0000') {
    const globalHandler = global();

    const roles = ref(settings.Roles);
    const newRoleName = ref('');
    const newRoleColor = ref(defaultColor);
    const getRoleName = (r) => r?.Name ?? 'Без Роли';
    const getRoleColor = (r) => r?.Color ?? defaultColor;
    const getRoles = () => roles;

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

    return {
        roles,
        newRoleName,
        newRoleColor,
        getRoleName,
        getRoleColor,
        appendCustomRole
    };
};

function playersHandle(defaultRole) {
    const globalHandler = global();

    const players = ref(settings.Players);
    const playersCount = ref(players.value.length);

    watch(playersCount, (newValue, oldValue) => {
        const diff = newValue - oldValue;
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
            "Role": defaultRole
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

    return {
        players,
        playersCount,
        changePlayerRole
    }
};

createApp({
    setup() {
        const rolesHandler = rolesHandle();
        const playerHandler = playersHandle(rolesHandler.roles.value[3]);
        const globalHandler = global();
        const timeDelay = ref(settings.TimeDelay);


        let modalInstance = null;

        onMounted(() => {
            const modalElement = document.getElementById('settingsModal');
            if (modalElement) {
                modalInstance = new bootstrap.Modal(modalElement);
                modalInstance.show();
            }
        });

        async function startGame() {
            if (timeDelay.value <= 0) {
                alert('Время на ходы должно быть больше 0');

                return;
            }

            if (playerHandler.playersCount.value <= 0) {
                alert('Должен быть хотя бы один игрок');

                return;
            }

            if (rolesHandler.roles.value.length === 0) {
                alert('Должна быть хотя бы одна роль');

                return;
            }

            try {
                const response = await fetch('https://localhost:7001/api/game/create', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(settings)
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
            ...globalHandler,
            ...rolesHandler,
            ...playerHandler
        };
    }
}).mount('#settingsModal');