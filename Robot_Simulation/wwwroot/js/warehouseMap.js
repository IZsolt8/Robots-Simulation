(function() {
    let TILE_SIZE = 64;
    const GRID_COLS = 20;
    const GRID_ROWS = 12;

    const chargingSlots = [
        { x: 0, y: 0 },
        { x: 1, y: 0 },
        { x: 0, y: 1 },
        { x: 1, y: 1 }
    ];

    const staticTextures = {
        floor: new Image(),
        packages: new Image(),
        shelf: new Image()
    };

    let staticLoaded = 0;
    const totalStatic = 3;

    function initStaticTextures() {
        staticTextures.floor.src = '/texture/floor.png';
        staticTextures.packages.src = '/texture/packages.png';
        staticTextures.shelf.src = '/texture/storage.png';
        for (let key in staticTextures) {
            staticTextures[key].onload = () => { staticLoaded++; };
            staticTextures[key].onerror = () => { staticLoaded++; };
        }
    }
    initStaticTextures();

    const textureCache = {};

    function getOrLoadTexture(modelFile) {
        if (!modelFile) return null;
        if (textureCache[modelFile]) return textureCache[modelFile];
        const img = new Image();
        img.src = '/texture/' + modelFile + '.png';
        textureCache[modelFile] = img;
        return img;
    }

    const mapGrid = [
        [3, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2],
        [3, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2],
        [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
        [0, 0, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 0, 0],
        [0, 0, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 0, 0],
        [0, 0, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 0, 0],
        [0, 0, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 0, 0],
        [0, 0, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 0, 0],
        [0, 0, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 0, 0],
        [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
        [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
        [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]
    ];

    const packagePoint = { x: 18, y: 0.5 };
    const shelfPoints = [
        { x: 3.5, y: 2 }, { x: 6.5, y: 2 }, { x: 9.5, y: 2 }, { x: 12.5, y: 2 }, { x: 15.5, y: 2 },
        { x: 3.5, y: 9 }, { x: 6.5, y: 9 }, { x: 9.5, y: 9 }, { x: 12.5, y: 9 }, { x: 15.5, y: 9 }
    ];

    let gameId = new URLSearchParams(window.location.search).get('id') || 'default';
    const sessionKey = 'robotPositions_' + gameId;
    let robots = {};

    try {
        let saved = sessionStorage.getItem(sessionKey);
        if (saved) robots = JSON.parse(saved);
    } catch(e) {}

    function getRobotData() {
        const scriptTag = document.getElementById('robot-data-json');
        if (scriptTag) {
            try { return JSON.parse(scriptTag.textContent); }
            catch (e) { return []; }
        }
        return [];
    }

    function getGameData() {
        const scriptTag = document.getElementById('game-data-json');
        if (scriptTag) {
            try { return JSON.parse(scriptTag.textContent); }
            catch (e) { return { HasPackagesToPack: false }; }
        }
        return { HasPackagesToPack: false };
    }

    function updateRobotTargets(serverRobots) {
        let currentRobotIds = new Set(serverRobots.map(r => r.Id.toString()));
        for (let id in robots) {
            if (!currentRobotIds.has(id.toString())) {
                delete robots[id];
            }
        }

        let hasPackages = getGameData().HasPackagesToPack;

        serverRobots.forEach(sr => {
            const slotIdx = sr.ChargingSlot >= 0 ? sr.ChargingSlot : 0;
            const slot = chargingSlots[Math.min(slotIdx, chargingSlots.length - 1)];

            if (!robots[sr.Id]) {
                robots[sr.Id] = {
                    id: sr.Id,
                    name: sr.Name,
                    modelFile: sr.ModelFile,
                    type: sr.Type,
                    x: sr.Type === 'charging' ? slot.x : Math.random() * (GRID_COLS - 1),
                    y: sr.Type === 'charging' ? slot.y : Math.random() * (GRID_ROWS - 1),
                    targetX: 0,
                    targetY: 0,
                    isCharging: sr.IsCharging,
                    state: sr.Type === 'charging' ? 'chargingStation' : 'idle',
                    battery: sr.BatteryLevel,
                    maxBattery: sr.MaxBattery,
                    chargingSlot: slotIdx
                };
                robots[sr.Id].targetX = robots[sr.Id].x;
                robots[sr.Id].targetY = robots[sr.Id].y;
            }

            const r = robots[sr.Id];
            r.name = sr.Name;
            r.modelFile = sr.ModelFile;
            r.isCharging = sr.IsCharging;
            r.battery = sr.BatteryLevel;
            r.maxBattery = sr.MaxBattery;
            r.chargingSlot = slotIdx;

            if (r.type === 'charging') {
                r.x = slot.x;
                r.y = slot.y;
                r.targetX = slot.x;
                r.targetY = slot.y;
                return;
            }

            if (r.isCharging) {
                r.state = 'charging';
                r.targetX = chargingSlots[0].x;
                r.targetY = chargingSlots[0].y;
            } else {
                let atTarget = Math.abs(r.x - r.targetX) < 0.2 && Math.abs(r.y - r.targetY) < 0.2;
                
                if (!hasPackages && r.state !== 'toShelf') {
                    if (r.state !== 'idle') {
                        r.state = 'idle';
                        r.targetX = r.x;
                        r.targetY = r.y;
                    }
                } else {
                    if (atTarget || r.state === 'idle' || r.state === 'charging') {
                        if (r.state === 'toPackage' && atTarget) {
                            r.state = 'toShelf';
                            let shelf = shelfPoints[Math.floor(Math.random() * shelfPoints.length)];
                            r.targetX = shelf.x;
                            r.targetY = shelf.y;
                        } else if (r.state === 'toShelf' && atTarget) {
                            if (!hasPackages) {
                                r.state = 'idle';
                                r.targetX = r.x;
                                r.targetY = r.y;
                            } else {
                                r.state = 'toPackage';
                                r.targetX = packagePoint.x;
                                r.targetY = packagePoint.y + Math.random() * 2;
                            }
                        } else if (r.state === 'idle' || r.state === 'charging') {
                            if (hasPackages) {
                                r.state = 'toPackage';
                                r.targetX = packagePoint.x;
                                r.targetY = packagePoint.y + Math.random() * 2;
                            }
                        }
                    }
                }
            }
        });

        sessionStorage.setItem(sessionKey, JSON.stringify(robots));
    }

    function drawGrid(ctx) {
        for (let y = 0; y < GRID_ROWS; y++) {
            for (let x = 0; x < GRID_COLS; x++) {
                const px = x * TILE_W;
                const py = y * TILE_H;

                if (staticLoaded < totalStatic) {
                    ctx.fillStyle = '#34495e';
                    ctx.fillRect(px, py, TILE_W, TILE_H);
                    ctx.strokeStyle = '#2c3e50';
                    ctx.strokeRect(px, py, TILE_W, TILE_H);
                    continue;
                }

                ctx.drawImage(staticTextures.floor, px, py, TILE_W, TILE_H);

                let tileType = mapGrid[y][x];
                if (tileType === 1) {
                    ctx.drawImage(staticTextures.shelf, px, py, TILE_W, TILE_H);
                } else if (tileType === 2) {
                    ctx.drawImage(staticTextures.packages, px, py, TILE_W, TILE_H);
                } else if (tileType === 3) {
                    ctx.fillStyle = 'rgba(46, 204, 113, 0.2)';
                    ctx.fillRect(px, py, TILE_W, TILE_H);
                }
            }
        }
    }

    function moveRobots() {
        const autoKey = sessionStorage.getItem(
            Object.keys(sessionStorage).find(k => k.startsWith('autoDay_'))
        );
        if (autoKey !== "true") return;

        const speed = 0.05;
        for (let id in robots) {
            let r = robots[id];
            if (r.type === 'charging') continue;

            let dx = r.targetX - r.x;
            let dy = r.targetY - r.y;
            let dist = Math.sqrt(dx * dx + dy * dy);

            if (dist > 0.05) {
                r.x += (dx / dist) * speed;
                r.y += (dy / dist) * speed;
            } else {
                r.x = r.targetX;
                r.y = r.targetY;
            }
        }
    }

    function drawRobots(ctx) {
        const chargingBySlot = {};
        for (let id in robots) {
            const r = robots[id];
            if (r.type !== 'charging') continue;
            const slotKey = r.chargingSlot;
            if (!chargingBySlot[slotKey]) chargingBySlot[slotKey] = [];
            chargingBySlot[slotKey].push(r);
        }

        for (let slotKey in chargingBySlot) {
            const group = chargingBySlot[slotKey];
            const rep = group[0];
            const slot = chargingSlots[Math.min(parseInt(slotKey), chargingSlots.length - 1)];
            const px = slot.x * TILE_W;
            const py = slot.y * TILE_H;
            const iconSize = TILE_SIZE;

            const robotImg = getOrLoadTexture(rep.modelFile);

            if (robotImg && robotImg.complete && robotImg.naturalWidth > 0) {
                ctx.drawImage(robotImg, px, py, iconSize, iconSize);
            } else {
                ctx.fillStyle = '#f39c12';
                ctx.beginPath();
                ctx.arc(px + iconSize / 2, py + iconSize / 2, iconSize / 2 - 4, 0, Math.PI * 2);
                ctx.fill();
            }


        }

        for (let id in robots) {
            const r = robots[id];
            if (r.type !== 'packing') continue;

            const px = r.x * TILE_W;
            const py = r.y * TILE_H;
            const iconSize = TILE_SIZE;

            const robotImg = getOrLoadTexture(r.modelFile);

            if (robotImg && robotImg.complete && robotImg.naturalWidth > 0) {
                ctx.drawImage(robotImg, px, py, iconSize, iconSize);
            } else {
                ctx.fillStyle = '#3498db';
                ctx.beginPath();
                ctx.arc(px + iconSize / 2, py + iconSize / 2, iconSize / 2 - 4, 0, Math.PI * 2);
                ctx.fill();
            }

            if (r.isCharging) {
                ctx.fillStyle = '#f1c40f';
                ctx.beginPath();
                ctx.arc(px + iconSize - 10, py + 10, 5, 0, Math.PI * 2);
                ctx.fill();
            }
        }


        ctx.textAlign = 'left';
        ctx.textBaseline = 'alphabetic';
    }

    let lastDataFetch = 0;
    let TILE_W = 64;
    let TILE_H = 64;

    function renderLoop() {
        const canvas = document.getElementById('warehouseCanvas');
        if (canvas) {
            if (canvas.width !== canvas.clientWidth || canvas.height !== canvas.clientHeight) {
                canvas.width  = canvas.clientWidth;
                canvas.height = canvas.clientHeight;
            }

            TILE_W = canvas.width  / GRID_COLS;
            TILE_H = canvas.height / GRID_ROWS;
            TILE_SIZE = Math.min(TILE_W, TILE_H);

            const ctx = canvas.getContext('2d');
            ctx.clearRect(0, 0, canvas.width, canvas.height);

            let now = Date.now();
            if (now - lastDataFetch > 1000) {
                let serverRobots = getRobotData();
                updateRobotTargets(serverRobots);
                lastDataFetch = now;
            }

            moveRobots();
            drawGrid(ctx);
            drawRobots(ctx);
        }

        requestAnimationFrame(renderLoop);
    }

    requestAnimationFrame(renderLoop);
})();
