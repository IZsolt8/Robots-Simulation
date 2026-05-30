(function() {
    let TILE_SIZE = 64;
    const GRID_COLS = 20;
    const GRID_ROWS = 12;
    
    const textures = {
        floor: new Image(),
        packages: new Image(),
        chargingRobot: new Image(),
        packingRobot: new Image(),
        shelf: new Image()
    };
    
    let texturesLoaded = 0;
    const totalTextures = 5;
    
    function initTextures() {
        textures.floor.src = '/texture/floor.png';
        textures.packages.src = '/texture/packages.png';
        textures.chargingRobot.src = '/texture/charging_robots.png';
        textures.packingRobot.src = '/texture/Packing_robot.png';
        textures.shelf.src = '/texture/storage.png';
        
        for (let key in textures) {
            textures[key].onload = () => {
                texturesLoaded++;
            };
        }
    }
    initTextures();
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
    const chargingPoint = { x: 0.5, y: 0.5 }; 
    const shelfPoints = [
        { x: 3.5, y: 2 }, { x: 6.5, y: 2 }, { x: 9.5, y: 2 }, { x: 12.5, y: 2 }, { x: 15.5, y: 2 },
        { x: 3.5, y: 9 }, { x: 6.5, y: 9 }, { x: 9.5, y: 9 }, { x: 12.5, y: 9 }, { x: 15.5, y: 9 }
    ];

    let gameId = new URLSearchParams(window.location.search).get('id') || 'default';
    const sessionKey = 'robotPositions_' + gameId;
    let robots = {}; 

    try {
        let saved = sessionStorage.getItem(sessionKey);
        if (saved) {
            robots = JSON.parse(saved);
        }
    } catch(e) {}

    function getRobotData() {
        const scriptTag = document.getElementById('robot-data-json');
        if (scriptTag) {
            try {
                return JSON.parse(scriptTag.textContent);
            } catch (e) {
                return [];
            }
        }
        return [];
    }

    function updateRobotTargets(serverRobots) {
        let currentRobotIds = new Set(serverRobots.map(r => r.Id.toString()));
        for (let id in robots) {
            if (!currentRobotIds.has(id.toString())) {
                delete robots[id];
            }
        }

        serverRobots.forEach(sr => {
            if (!robots[sr.Id]) {
                robots[sr.Id] = {
                    id: sr.Id,
                    type: sr.Type,
                    x: sr.Type === 'charging' ? chargingPoint.x : Math.random() * (GRID_COLS - 1),
                    y: sr.Type === 'charging' ? chargingPoint.y : Math.random() * (GRID_ROWS - 1),
                    targetX: 0,
                    targetY: 0,
                    isCharging: sr.IsCharging,
                    state: sr.Type === 'charging' ? 'chargingStation' : 'idle',
                    battery: sr.BatteryLevel,
                    maxBattery: sr.MaxBattery
                };
                robots[sr.Id].targetX = robots[sr.Id].x;
                robots[sr.Id].targetY = robots[sr.Id].y;
            }

            const r = robots[sr.Id];
            r.isCharging = sr.IsCharging;
            r.battery = sr.BatteryLevel;
            r.maxBattery = sr.MaxBattery;

            if (r.type === 'charging') {
                r.x = chargingPoint.x;
                r.y = chargingPoint.y;
                r.targetX = chargingPoint.x;
                r.targetY = chargingPoint.y;
                return;
            }

            if (r.isCharging) {
                r.state = 'toCharger';
                r.targetX = chargingPoint.x + Math.random() - 0.5;
                r.targetY = chargingPoint.y + Math.random() - 0.5;
            } else {
                let atTarget = Math.abs(r.x - r.targetX) < 0.2 && Math.abs(r.y - r.targetY) < 0.2;
                
                if (atTarget || r.state === 'idle' || r.state === 'toCharger') {
                    if (r.state === 'toPackage' && atTarget) {
                        r.state = 'toShelf';
                        let shelf = shelfPoints[Math.floor(Math.random() * shelfPoints.length)];
                        r.targetX = shelf.x;
                        r.targetY = shelf.y;
                    } else if (r.state === 'toShelf' && atTarget) {
                        r.state = 'toPackage';
                        r.targetX = packagePoint.x;
                        r.targetY = packagePoint.y + Math.random() * 2;
                    } else if (r.state === 'idle' || r.state === 'toCharger') {
                        r.state = 'toPackage';
                        r.targetX = packagePoint.x;
                        r.targetY = packagePoint.y + Math.random() * 2;
                    }
                }
            }
        });
        sessionStorage.setItem(sessionKey, JSON.stringify(robots));
    }

    function drawGrid(ctx) {
        for (let y = 0; y < GRID_ROWS; y++) {
            for (let x = 0; x < GRID_COLS; x++) {
                if (texturesLoaded < totalTextures) {
                    ctx.fillStyle = '#34495e';
                    ctx.fillRect(x * TILE_SIZE, y * TILE_SIZE, TILE_SIZE, TILE_SIZE);
                    ctx.strokeStyle = '#2c3e50';
                    ctx.strokeRect(x * TILE_SIZE, y * TILE_SIZE, TILE_SIZE, TILE_SIZE);
                    continue;
                }

                ctx.drawImage(textures.floor, x * TILE_SIZE, y * TILE_SIZE, TILE_SIZE, TILE_SIZE);
                
                let tileType = mapGrid[y][x];
                if (tileType === 1) {
                    ctx.drawImage(textures.shelf, x * TILE_SIZE, y * TILE_SIZE, TILE_SIZE, TILE_SIZE);
                } else if (tileType === 2) {
                    ctx.drawImage(textures.packages, x * TILE_SIZE, y * TILE_SIZE, TILE_SIZE, TILE_SIZE);
                } else if (tileType === 3) {
                    ctx.fillStyle = 'rgba(46, 204, 113, 0.2)';
                    ctx.fillRect(x * TILE_SIZE, y * TILE_SIZE, TILE_SIZE, TILE_SIZE);
                }
            }
        }
    }

    function moveRobots() {
        const autoKey = sessionStorage.getItem(Object.keys(sessionStorage).find(k => k.startsWith('autoDay_')));
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
        for (let id in robots) {
            let r = robots[id];
            let px = r.x * TILE_SIZE;
            let py = r.y * TILE_SIZE;

            if (texturesLoaded >= totalTextures) {
                if (r.type === 'charging') {
                    ctx.drawImage(textures.chargingRobot, px, py, TILE_SIZE, TILE_SIZE);
                } else {
                    ctx.drawImage(textures.packingRobot, px, py, TILE_SIZE, TILE_SIZE);
                    
                    if (r.maxBattery > 0) {
                        let pct = Math.max(0, Math.min(1, r.battery / r.maxBattery));
                        ctx.fillStyle = 'red';
                        ctx.fillRect(px + 4, py - 6, TILE_SIZE - 8, 4);
                        ctx.fillStyle = '#2ecc71';
                        ctx.fillRect(px + 4, py - 6, (TILE_SIZE - 8) * pct, 4);
                    }

                    if (r.isCharging) {
                        ctx.fillStyle = '#f1c40f';
                        ctx.beginPath();
                        ctx.arc(px + TILE_SIZE - 10, py + 10, 5, 0, Math.PI * 2);
                        ctx.fill();
                    }
                }
            } else {
                ctx.fillStyle = r.type === 'charging' ? '#f39c12' : '#3498db';
                ctx.beginPath();
                ctx.arc(px + TILE_SIZE/2, py + TILE_SIZE/2, TILE_SIZE/2 - 4, 0, Math.PI * 2);
                ctx.fill();
            }
        }
    }

    let lastDataFetch = 0;
    function renderLoop() {
        const canvas = document.getElementById('warehouseCanvas');
        if (canvas) {
            if (canvas.width !== canvas.clientWidth || canvas.height !== canvas.clientHeight) {
                canvas.width = canvas.clientWidth;
                canvas.height = canvas.clientHeight;
            }

            TILE_SIZE = Math.min(canvas.width / GRID_COLS, canvas.height / GRID_ROWS);
            const ctx = canvas.getContext('2d');
            ctx.clearRect(0, 0, canvas.width, canvas.height);
            const offsetX = (canvas.width - TILE_SIZE * GRID_COLS) / 2;
            const offsetY = (canvas.height - TILE_SIZE * GRID_ROWS) / 2;
            ctx.save();
            ctx.translate(offsetX, offsetY);
            let now = Date.now();
            if (now - lastDataFetch > 1000) {
                let serverRobots = getRobotData();
                updateRobotTargets(serverRobots);
                lastDataFetch = now;
            }

            moveRobots();
            drawGrid(ctx);
            drawRobots(ctx);

            ctx.restore();
        }
        
        requestAnimationFrame(renderLoop);
    }

    requestAnimationFrame(renderLoop);
})();