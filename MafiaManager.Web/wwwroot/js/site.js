window.addEventListener('onGameStarted', (event) => {
    const newGame = event.detail;
    window.game = newGame;
});