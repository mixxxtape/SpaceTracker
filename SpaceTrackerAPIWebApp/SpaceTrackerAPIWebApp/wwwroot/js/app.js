// ===== STATE =====
let currentUser = null;
let currentApod = null;
let savedFavoriteId = null;
let issMap = null;
let issMarker = null;
let issInterval = null;

// ===== STARS BACKGROUND =====
(function createStars() {
    const container = document.getElementById('stars');
    for (let i = 0; i < 180; i++) {
        const s = document.createElement('div');
        s.className = 'star';
        const size = Math.random() * 2.5 + 0.5;
        s.style.cssText = `
            width:${size}px; height:${size}px;
            top:${Math.random() * 100}%; left:${Math.random() * 100}%;
            --d:${(Math.random() * 4 + 2).toFixed(1)}s;
            --delay:-${(Math.random() * 4).toFixed(1)}s;
        `;
        container.appendChild(s);
    }
})();

// ===== AUTH =====
function showTab(tab) {
    document.querySelectorAll('.auth-tab').forEach(t => t.classList.remove('active'));
    event.target.classList.add('active');
    document.getElementById('login-form').style.display = tab === 'login' ? 'block' : 'none';
    document.getElementById('register-form').style.display = tab === 'register' ? 'block' : 'none';
    document.getElementById('auth-msg').textContent = '';
}

async function login() {
    const email = document.getElementById('login-email').value.trim();
    const password = document.getElementById('login-password').value;
    const msg = document.getElementById('auth-msg');

    if (!email || !password) { msg.textContent = 'Заповніть всі поля'; return; }

    try {
        const res = await fetch('/api/auth/login', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ email, password })
        });
        if (res.ok) {
            const user = await res.json();
            setUser(user);
        } else {
            const err = await res.text();
            msg.textContent = err || 'Невірний email або пароль';
        }
    } catch {
        // fallback for demo without real auth
        msg.textContent = 'Помилка з\'єднання — вхід як гість';
        setTimeout(() => continueAsGuest(), 1000);
    }
}

async function register() {
    const username = document.getElementById('reg-username').value.trim();
    const email = document.getElementById('reg-email').value.trim();
    const password = document.getElementById('reg-password').value;
    const msg = document.getElementById('auth-msg');

    if (!username || !email || !password) { msg.textContent = 'Заповніть всі поля'; return; }

    try {
        const res = await fetch('/api/auth/register', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ username, email, password })
        });
        if (res.ok) {
            const user = await res.json();
            setUser(user);
        } else {
            const err = await res.text();
            msg.textContent = err || 'Помилка реєстрації';
        }
    } catch {
        msg.textContent = 'Помилка з\'єднання — вхід як гість';
        setTimeout(() => continueAsGuest(), 1000);
    }
}

function continueAsGuest() {
    setUser({ id: 0, username: 'ГІСТЬ', email: '' });
}

function setUser(user) {
    currentUser = user;
    document.getElementById('auth-overlay').classList.remove('active');
    document.getElementById('app').style.display = 'block';
    document.getElementById('user-label').textContent = user.username || 'ГІСТЬ';

    // Init default date
    const today = new Date().toISOString().split('T')[0];
    document.getElementById('apod-date').value = today;
    const weekAgo = new Date(Date.now() - 7 * 86400000).toISOString().split('T')[0];
    document.getElementById('ast-start').value = weekAgo;
    document.getElementById('ast-end').value = today;

    loadApod();
    initIssMap();
}

function logout() {
    currentUser = null;
    currentApod = null;
    if (issInterval) clearInterval(issInterval);
    document.getElementById('app').style.display = 'none';
    document.getElementById('auth-overlay').classList.add('active');
    document.getElementById('auth-msg').textContent = '';
}

// ===== NAVIGATION =====
function showSection(name) {
    document.querySelectorAll('.section').forEach(s => s.classList.remove('active'));
    document.querySelectorAll('.nav-btn').forEach(b => b.classList.remove('active'));
    document.getElementById('section-' + name).classList.add('active');
    event.currentTarget.classList.add('active');

    if (name === 'favorites') loadFavorites();
    if (name === 'iss') {
        setTimeout(() => { if (issMap) issMap.invalidateSize(); }, 100);
        refreshIss();
        if (!issInterval) issInterval = setInterval(refreshIss, 10000);
    } else {
        if (issInterval) { clearInterval(issInterval); issInterval = null; }
    }
}

// ===== APOD =====
async function loadApod() {
    const date = document.getElementById('apod-date').value;
    showLoading('apod', true);
    document.getElementById('apod-content').style.display = 'none';

    try {
        const url = date ? `/api/apod?date=${date}` : '/api/apod';
        const res = await fetch(url);
        const data = await res.json();
        currentApod = data;
        savedFavoriteId = null;

        document.getElementById('apod-title').textContent = data.title || 'Без назви';
        document.getElementById('apod-date-display').textContent = data.date || date || '';
        document.getElementById('apod-description').textContent = data.explanation || '';

        const img = document.getElementById('apod-image');
        if (data.media_type === 'video') {
            img.src = `https://img.youtube.com/vi/${extractYoutubeId(data.url)}/hqdefault.jpg`;
        } else {
            img.src = data.hdurl || data.url || '';
        }

        const favBtn = document.getElementById('fav-btn');
        favBtn.textContent = '⭐ ЗБЕРЕГТИ';
        favBtn.classList.remove('saved');

        showLoading('apod', false);
        document.getElementById('apod-content').style.display = 'grid';
    } catch (err) {
        showLoading('apod', false);
        console.error('APOD error:', err);
    }
}

function extractYoutubeId(url) {
    const m = url.match(/(?:embed\/|v=|youtu\.be\/)([^&?/]+)/);
    return m ? m[1] : '';
}

async function toggleFavorite() {
    if (!currentApod) return;
    const btn = document.getElementById('fav-btn');

    if (savedFavoriteId) {
        // remove
        try {
            await fetch(`/api/favorites/${savedFavoriteId}`, { method: 'DELETE' });
            savedFavoriteId = null;
            btn.textContent = '⭐ ЗБЕРЕГТИ';
            btn.classList.remove('saved');
        } catch (err) { console.error(err); }
    } else {
        // add
        try {
            const body = {
                userId: currentUser?.id || 0,
                title: currentApod.title,
                imageUrl: currentApod.url,
                nasaDate: currentApod.date
            };
            const res = await fetch('/api/favorites', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(body)
            });
            if (res.ok) {
                const fav = await res.json();
                savedFavoriteId = fav.id;
                btn.textContent = '★ ЗБЕРЕЖЕНО';
                btn.classList.add('saved');
            }
        } catch (err) { console.error(err); }
    }
}

// ===== ASTEROIDS =====
async function loadAsteroids() {
    const start = document.getElementById('ast-start').value;
    const end = document.getElementById('ast-end').value;
    if (!start || !end) return;

    showLoading('asteroids', true);
    document.getElementById('asteroids-list').innerHTML = '';

    try {
        const res = await fetch(`/api/asteroids?startDate=${start}&endDate=${end}`);
        const raw = await res.json();

        // NASA NeoWs returns nested by date
        let asteroids = [];
        if (raw.near_earth_objects) {
            Object.values(raw.near_earth_objects).forEach(dayArr => {
                asteroids = asteroids.concat(dayArr);
            });
        } else if (Array.isArray(raw)) {
            asteroids = raw;
        }

        // Sort by close approach date
        asteroids.sort((a, b) => {
            const da = a.close_approach_data?.[0]?.close_approach_date || '';
            const db = b.close_approach_data?.[0]?.close_approach_date || '';
            return da.localeCompare(db);
        });

        showLoading('asteroids', false);

        const grid = document.getElementById('asteroids-list');
        if (asteroids.length === 0) {
            grid.innerHTML = '<p style="color:var(--text-dim);font-family:Orbitron,sans-serif;font-size:12px;">Нічого не знайдено</p>';
            return;
        }

        asteroids.forEach(ast => {
            grid.appendChild(buildAsteroidCard(ast));
        });
    } catch (err) {
        showLoading('asteroids', false);
        console.error('Asteroids error:', err);
    }
}

function buildAsteroidCard(ast) {
    const card = document.createElement('div');
    card.className = 'asteroid-card' + (ast.is_potentially_hazardous_asteroid ? ' hazardous' : '');

    const approach = ast.close_approach_data?.[0] || {};
    const approachDate = approach.close_approach_date || '—';
    const distKm = approach.miss_distance?.kilometers
        ? parseFloat(approach.miss_distance.kilometers).toLocaleString('uk-UA', { maximumFractionDigits: 0 }) + ' км'
        : '—';
    const speed = approach.relative_velocity?.kilometers_per_hour
        ? parseFloat(approach.relative_velocity.kilometers_per_hour).toLocaleString('uk-UA', { maximumFractionDigits: 0 }) + ' км/год'
        : '—';
    const dMin = ast.estimated_diameter?.kilometers?.estimated_diameter_min;
    const dMax = ast.estimated_diameter?.kilometers?.estimated_diameter_max;
    const diameter = dMin != null
        ? `${(dMin * 1000).toFixed(0)}–${(dMax * 1000).toFixed(0)} м`
        : '—';

    const countdown = getCountdown(approachDate);

    card.innerHTML = `
        <div class="asteroid-name">${ast.name || 'Невідомий'}</div>
        <div class="asteroid-stats">
            <div class="ast-stat">
                <div class="ast-stat-label">ВІДСТАНЬ</div>
                <div class="ast-stat-value" style="font-size:13px">${distKm}</div>
            </div>
            <div class="ast-stat">
                <div class="ast-stat-label">ШВИДКІСТЬ</div>
                <div class="ast-stat-value" style="font-size:13px">${speed}</div>
            </div>
            <div class="ast-stat">
                <div class="ast-stat-label">ДІАМЕТР</div>
                <div class="ast-stat-value" style="font-size:13px">${diameter}</div>
            </div>
            <div class="ast-stat">
                <div class="ast-stat-label">ДАТА ЗБЛИЖЕННЯ</div>
                <div class="ast-stat-value" style="font-size:13px">${approachDate}</div>
            </div>
        </div>
        <div class="countdown">
            <div class="countdown-label">ВІДЛІК ДО ЗБЛИЖЕННЯ</div>
            <div class="countdown-timer">${countdown}</div>
        </div>
    `;
    return card;
}

function getCountdown(dateStr) {
    if (!dateStr || dateStr === '—') return '—';
    const target = new Date(dateStr);
    const now = new Date();
    const diff = target - now;

    if (diff < 0) return 'МИНУЛО';
    const days = Math.floor(diff / 86400000);
    const hours = Math.floor((diff % 86400000) / 3600000);
    const mins = Math.floor((diff % 3600000) / 60000);
    return `${days}д ${hours}г ${mins}хв`;
}

// ===== ISS MAP =====
function initIssMap() {
    if (issMap) return;
    issMap = L.map('iss-map', { zoomControl: true, attributionControl: false }).setView([0, 0], 2);
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '© OpenStreetMap'
    }).addTo(issMap);

    const issIcon = L.divIcon({
        html: '<div style="font-size:28px;line-height:1;filter:drop-shadow(0 0 8px #00d4ff);">🛸</div>',
        className: '',
        iconSize: [32, 32],
        iconAnchor: [16, 16]
    });
    issMarker = L.marker([0, 0], { icon: issIcon }).addTo(issMap);
    refreshIss();
}

async function refreshIss() {
    try {
        const res = await fetch('/api/iss/position');
        const data = await res.json();

        const lat = parseFloat(data.latitude ?? data.lat ?? 0);
        const lon = parseFloat(data.longitude ?? data.lon ?? 0);
        const speed = data.velocity ? parseFloat(data.velocity).toFixed(0) + ' км/год' : '—';
        const alt = data.altitude ? parseFloat(data.altitude).toFixed(1) + ' км' : '—';

        document.getElementById('iss-lat').textContent = lat.toFixed(4) + '°';
        document.getElementById('iss-lon').textContent = lon.toFixed(4) + '°';
        document.getElementById('iss-speed').textContent = speed;
        document.getElementById('iss-alt').textContent = alt;

        if (issMarker) {
            issMarker.setLatLng([lat, lon]);
            issMap.panTo([lat, lon], { animate: true, duration: 1 });
        }
    } catch (err) {
        console.error('ISS error:', err);
    }
}

async function getPassTimes() {
    const lat = parseFloat(document.getElementById('pass-lat').value);
    const lon = parseFloat(document.getElementById('pass-lon').value);
    const result = document.getElementById('pass-result');
    result.innerHTML = '<span style="color:var(--text-dim)">Завантаження...</span>';

    try {
        const res = await fetch(`/api/iss/pass?lat=${lat}&lon=${lon}`);
        const data = await res.json();

        const passes = data.response || data.passes || [];
        if (!passes.length) {
            result.innerHTML = '<span style="color:var(--text-dim)">Дані недоступні</span>';
            return;
        }

        result.innerHTML = passes.slice(0, 5).map(p => {
            const rise = new Date((p.risetime || p.rise_time) * 1000).toLocaleString('uk-UA');
            const dur = p.duration ? `${p.duration} с` : '';
            return `<div class="pass-time-row"><span>${rise}</span><span style="color:var(--accent)">${dur}</span></div>`;
        }).join('');
    } catch (err) {
        result.innerHTML = '<span style="color:var(--text-dim)">Сервіс тимчасово недоступний</span>';
    }
}

// ===== FAVORITES =====
async function loadFavorites() {
    showLoading('favorites', true);
    document.getElementById('favorites-grid').innerHTML = '';
    document.getElementById('no-favorites').style.display = 'none';

    try {
        const res = await fetch('/api/favorites');
        const favs = await res.json();

        showLoading('favorites', false);
        const grid = document.getElementById('favorites-grid');

        // Filter by current user if logged in (not guest)
        const userFavs = currentUser?.id
            ? favs.filter(f => f.userId === currentUser.id || f.userId === 0)
            : favs;

        if (userFavs.length === 0) {
            document.getElementById('no-favorites').style.display = 'block';
            return;
        }

        userFavs.forEach(fav => {
            const card = document.createElement('div');
            card.className = 'fav-card';
            card.innerHTML = `
                <img class="fav-img" src="${fav.imageUrl || ''}" alt="${fav.title}" loading="lazy">
                <div class="fav-body">
                    <div class="fav-title">${fav.title || 'Без назви'}</div>
                    <div class="fav-date">${fav.nasaDate || ''}</div>
                    <button class="btn-delete" onclick="deleteFavorite(${fav.id}, this)">✕ ВИДАЛИТИ</button>
                </div>
            `;
            grid.appendChild(card);
        });
    } catch (err) {
        showLoading('favorites', false);
        console.error('Favorites error:', err);
    }
}

async function deleteFavorite(id, btn) {
    try {
        const res = await fetch(`/api/favorites/${id}`, { method: 'DELETE' });
        if (res.ok) {
            const card = btn.closest('.fav-card');
            card.style.opacity = '0';
            card.style.transform = 'scale(0.9)';
            card.style.transition = 'all 0.3s';
            setTimeout(() => { card.remove(); checkEmpty(); }, 300);
        }
    } catch (err) { console.error(err); }
}

function checkEmpty() {
    const grid = document.getElementById('favorites-grid');
    if (!grid.children.length) {
        document.getElementById('no-favorites').style.display = 'block';
    }
}

// ===== HELPERS =====
function showLoading(section, show) {
    const el = document.getElementById(section + '-loading');
    if (!el) return;
    el.classList.toggle('active', show);
}
