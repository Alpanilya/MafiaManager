import { createApp, ref, watch, onMounted, computed } from 'https://unpkg.com/vue@3/dist/vue.esm-browser.prod.js'
import { globalHandle, rolesHandle, playersHandle, gameSettingsHandle } from './settings.js';

function gameHandle() {
    const globalHandler = globalHandle();
    const rolesHandler = rolesHandle([]);
    const playerHandler = playersHandle([]);
    const currentGame = ref({});
    const settings = ref({});

    window.addEventListener('onGameStarted', e => {
        const newGame = event.detail;
        currentGame.value = newGame

        const gameSettings = newGame.gameSettings ?? newGame.GameSettings;

        settings.value = gameSettings;
    });

    function getGamePlayers() {
        return settings.value?.players;
    }

    function getRolesStats() {
        const players = getGamePlayers();

        if (!players)
            return;

        settings.value

        return null;
    }

    return {
        currentGame,
        getGamePlayers,
        getRolesStats
    }
}

createApp({
    setup() {
        const game = gameHandle();
        const gameSettings = gameSettingsHandle();
        const globalHandler = globalHandle();

        onMounted(async () => {
            await gameSettings.setDefaultSettings();
        });

        return {
            game,
            gameSettings,
            ...globalHandler
        };
    }
}).mount('#gameBoard');