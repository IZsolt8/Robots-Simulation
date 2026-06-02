window.triggerAchievementPopups = async function(newAchievementIds) {
    if (!newAchievementIds || newAchievementIds.length === 0) return;

    if (!window.achievementsData) {
        try {
            let response = await fetch('/data/achievements.json');
            let data = await response.json();
            window.achievementsData = data.achievements;
        } catch (e) {
            console.error("Error loading achievements:", e);
            return;
        }
    }
    
    let achievements = window.achievementsData;
    
    for (let i = 0; i < newAchievementIds.length; i++) {
        let achId = newAchievementIds[i];
        let ach = achievements.find(a => a.id === achId);
        if (ach) {
            await showPopup(ach);
        }
    }

    if (window.renderTrophies) {
        window.renderTrophies();
    }
};

function showPopup(ach) {
    return new Promise(resolve => {
        let overlay = document.createElement('div');
        overlay.style.position = 'fixed';
        overlay.style.top = '0';
        overlay.style.left = '0';
        overlay.style.width = '100vw';
        overlay.style.height = '100vh';
        overlay.style.backgroundColor = 'rgba(0, 0, 0, 0.7)';
        overlay.style.zIndex = '9999';
        overlay.style.display = 'flex';
        overlay.style.alignItems = 'center';
        overlay.style.justifyContent = 'center';
        overlay.id = "popup_" + ach.id;
        
        let popup = document.createElement('div');
        popup.style.backgroundColor = '#192045';
        popup.style.border = '2px solid #2ecc71';
        popup.style.borderRadius = '10px';
        popup.style.padding = '30px';
        popup.style.textAlign = 'center';
        popup.style.color = '#ecf0f1';
        popup.style.boxShadow = '0 0 20px rgba(46, 204, 113, 0.5)';
        popup.style.maxWidth = '400px';
        
        let title = document.createElement('h2');
        title.innerText = "Gratulálok szereztél egy trófeát!";
        title.style.fontSize = '20px';
        title.style.marginBottom = '20px';
        title.style.color = '#2ecc71';
        
        let img = document.createElement('img');
        img.src = `/achievements_image/${ach.img || (ach.id + '.png')}`;
        img.style.width = '100px';
        img.style.height = '100px';
        img.style.objectFit = 'contain';
        img.style.marginBottom = '15px';
        
        let achName = document.createElement('h3');
        achName.innerText = ach.title;
        achName.style.margin = '0';
        achName.style.fontSize = '24px';
        
        let btn = document.createElement('button');
        btn.innerText = "OK";
        btn.style.marginTop = '20px';
        btn.style.padding = '10px 30px';
        btn.style.backgroundColor = '#2ecc71';
        btn.style.color = '#fff';
        btn.style.border = 'none';
        btn.style.borderRadius = '5px';
        btn.style.cursor = 'pointer';
        btn.style.fontSize = '16px';
        btn.style.transition = 'background-color 0.3s';
        
        btn.onmouseover = () => btn.style.backgroundColor = '#27ae60';
        btn.onmouseout = () => btn.style.backgroundColor = '#2ecc71';
        
        btn.onclick = () => {
            document.body.removeChild(overlay);
            resolve();
        };
        
        popup.appendChild(title);
        popup.appendChild(img);
        popup.appendChild(achName);
        popup.appendChild(btn);
        
        overlay.appendChild(popup);
        document.body.appendChild(overlay);
    });
}
