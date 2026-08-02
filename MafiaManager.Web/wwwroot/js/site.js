let settingsModal;
let roles = [];
let settings = {};
let players = [];

const playerCountInput = document.getElementById('playerCount');
const timeDelayInput = document.getElementById('timeDelay');

playerCountInput.addEventListener('input', (e) => {
    let newValue = parseInt(e.target.value, 10);
    if (newValue > e.target.max) {
        e.target.value = e.target.max;
        return;
    }
    const prevValue = players.length;
    const diff = newValue - prevValue;
    if (diff > 0) {
        appendPlayers(diff);
    }
    else {
        removePlayers(Math.abs(diff));
    }
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
        players = settings.players || [];
        players.forEach(player => appendPlayer(player));
        playerCountInput.value = players.length;
        timeDelayInput.value = settings.timeDelay;
        renderRoles();
    }
    else {
        console.error("Ошибка загрузки настроек", response.statusText);
    }
}

function appendRole(role) {
    if (!role)
        return;

    const container = document.getElementById('modalRolesGrid');
    const div = document.createElement('div');

    div.className = 'd-flex justify-content-between bg-dark p-2 rounded border border-secondary';
    div.innerHTML = `
            <div class="d-flex align-items-center gap-2">
                <span style="width: 12px; height: 12px; border-radius: 50%; background-color: ${role.color}; display: inline-block;"></span>
                <small class="fw-semibold text-white">${role.name}</small>
            </div>
            <button type="button" class="btn-close btn-close-white btn-sm" onclick="removeRole(${role.id})"></button>
        `;
    container.appendChild(div);
}

function renderRoles() {
    const container = document.getElementById('modalRolesGrid');

    if (!container)
        return;

    container.innerHTML = '';
    roles.forEach(role => appendRole(role));

    const countEl = document.getElementById('rolesCount');
    if (countEl)
        countEl.innerText = `Всего: ${roles.length}`;
}

function appendCustomRole() {
    const nameInput = document.getElementById('newRoleName');
    const colorInput = document.getElementById('newRoleColor');
    if (!nameInput || !nameInput.value.trim())
        return;
    const id = getLastId(roles);
    const role = { name: nameInput.value.trim(), color: colorInput.value, id: id };
    roles.push(role);
    renderRoles();
    nameInput.value = '';
}

function removeRole(id) {
    const index = roles.findIndex(r => r.id === id);
    if (index !== -1) {
        roles.splice(index, 1);
        renderRoles();
    }
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

function appendPlayers(count) {
    for (let i = 1; i <= count; i++) {
        appendPlayer();
    }
}

function getLastId(array) {
    let lastIndex = 0;

    if (array.length == 0)
        return 1;

    return Math.max(...array.map(i => i.id)) + 1;
}

function appendPlayer(player = null) {
    const container = document.getElementById('playersGrid');

    const i = getLastId(players)

    const div = document.createElement('div');

    player = player || {
        "state": 0,
        "id": i,
        "name": `Игрок ${i}`,
        "role": roles[3]
    }

    players.push(player);
    div.className = 'col';
    div.innerHTML = `
            <div class="card bg-dark text-white border-secondary border-2 text-center p-1" style="border-color: ${player.role.color} !important;" id="card-body-${player.id}">
                <div class="display-6 fw-bold">${player.id}</div>
                <small class="text-uppercase mt-1">${player.name}</small>
                                <button class="btn btn-secondary btn-sm dropdown-toggle"
                        type="button"
                        data-bs-toggle="dropdown"
                        aria-expanded="false">
                    ${player.role ? player.role.name : 'Роль'}
                </button>
                <ul class="dropdown-menu dropdown-menu-dark" data-player-id="${player.id}">
                    ${roles.map(role => `
                        <li>
                            <a class="dropdown-item" href="#" data-role-id="${role.id}">
                                ${role.name}
                            </a>
                        </li>
                    `).join('')}
                </ul>
            </div>`;

    container.appendChild(div);

    document.addEventListener('click', (e) => {
        const item = e.target.closest('.dropdown-item');
        if (!item)
            return;

        e.preventDefault();
        const roleId = parseInt(item.dataset.roleId, 10);
        const dropdown = item.closest('.dropdown-menu');
        const playerId = parseInt(dropdown.dataset.playerId, 10);

        const player = players.find(p => p.id === playerId);
        const role = roles.find(r => r.id === roleId);

        if (!player || !role) return;

        player.role = role;

        const cardBody = document.getElementById(`card-body-${player.id}`);
        if (cardBody) {
            cardBody.style.setProperty('border-color', role.color, 'important');
        }

        const btn = dropdown.previousElementSibling;
        if (btn) {
            btn.textContent = role.name;
        }
    });
}

function removePlayers(count) {
    for (let i = 1; i <= count; i++) {
        removePlayer();
    }
}

function removePlayer() {
    const container = document.getElementById('playersGrid');
    const childs = container.children;
    const index = childs.length - 1;
    const child = childs[index];
    if (child) {
        container.removeChild(child);
        players.pop();
    }
}