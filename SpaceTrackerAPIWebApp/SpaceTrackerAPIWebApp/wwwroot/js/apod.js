async function loadApod() {
    try {
        const response = await fetch('/api/apod');
        const data = await response.json();

        document.getElementById('apod-title').textContent = data.title;
        document.getElementById('apod-image').src = data.url;
        document.getElementById('apod-description').textContent = data.explanation;
    } catch (error) {
        console.error('Помилка завантаження даних:', error);
    }
}

loadApod();