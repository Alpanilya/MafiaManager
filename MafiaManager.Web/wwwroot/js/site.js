let settingsModal;
let roles;
let settings;
let players = [];

const playerCountInput = document.getElementById('playerCount');
const timeDelayInput = document.getElementById('timeDelay');

playerCountInput.addEventListener('input', (e) => {
    settings.playerCount = parseInt(e.target.value, 10);
    appendRow()
});

timeDelayInput.addEventListener('input', (e) => {
    settings.timeDelay = parseInt(e.target.value, 10);
});


document.addEventListener('DOMContentLoaded', async () => {
    const modalElement = document.getElementById('settingsModal');
    if (modalElement) {
        settingsModal = new bootstrap.Modal(modalElement);
        await loadDefaultSettings();
        settingsModal.show();
    }
});

async function loadDefaultSettings() {
        const response = await fetch('https://localhost:7001/api/game/getGameSettings');
    if (response.ok) {
        settings = await response.json();
        roles = settings.roles;
        playerCountInput.value = settings.playerCount;
        timeDelayInput.value = settings.timeDelay;
        renderRoles();
    }
    else {
        console.error("Ошибка загрузки настроек", response.statusText);
    }
}

function renderRoles() {
    const container = document.getElementById('modalRolesList');
    if (!container)
        return;
    container.innerHTML = '';
    roles.forEach((role, idx) => {
        const div = document.createElement('div');
        div.className = 'd-flex justify-content-between bg-dark p-2 rounded border border-secondary';
        div.innerHTML = `
            <div class="d-flex align-items-center gap-2">
                <span style="width: 12px; height: 12px; border-radius: 50%; background-color: ${role.color}; display: inline-block;"></span>
                <small class="fw-semibold text-white">${role.name}</small>
            </div>
            ${!role.default ? `<button type="button" class="btn-close btn-close-white btn-sm" onclick="removeRole(${idx})"></button>` : ''}
        `;
        container.appendChild(div);
    });
    const countEl = document.getElementById('rolesCount');
    if (countEl)
        countEl.innerText = `Всего: ${roles.length}`;
}

function addCustomRole() {
    const nameInput = document.getElementById('newRoleName');
    const colorInput = document.getElementById('newRoleColor');
    if (!nameInput || !nameInput.value.trim())
        return;

    roles.push({ name: nameInput.value.trim(), color: colorInput.value, id: 0 });
    renderRoles();
    nameInput.value = '';
}

function removeRole(idx) {
    roles.splice(idx, 1);
    renderRoles();
}

async function startGame() {
    const count = parseInt(document.getElementById('playerCount').value);

    try {
        const response = await fetch('https://localhost:7001/api/game/create', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(settings)
        });

        if (response.ok) {
            settingsModal.hide();
            const grid = document.getElementById('playersGrid');
            grid.innerHTML = '';
            for (let i = 1; i <= count; i++) {
                grid.innerHTML += `
                        <div class="col">
                            <div class="card h-100 bg-dark text-white border-secondary border-2 text-center p-3">
                                <div class="display-6 fw-bold">${i}</div>
                                <small class="text-muted text-uppercase mt-1">Игрок ${i}</small>
                            </div>
                        </div>`;
            }
        }
    } catch (err) {
        console.error("Ошибка старта игры", err);
    }
}

function appendRow() {
    const container = document.getElementById('playersGrid');
    const i = container.children.length + 1;
    const div = document.createElement('div');
    players.push(
        {
            id: i,
            name: 'Игрок'
        });
    div.className = 'col';
    div.innerHTML = `
        <div class="col">
            <div class="card h-100 bg-dark text-white border-secondary border-2 text-center p-3">
                <div class="display-6 fw-bold">${i}</div>
                <small class="text-muted text-uppercase mt-1">Игрок ${i}</small>
            </div>
        </div>`;
    container.appendChild(div);
}