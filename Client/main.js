import { map, loadMap, getLonLat } from '/map.js'
window.onload = loadMap()
map.addEventListener('click', (e) => {
	const lon = (e.clientX - rect.left) * px2lon + topLeft.lon
	const lat = (e.clientY - rect.top) * px2lat + topLeft.lat
	const lonlat = getLonLat(lon, lat)
})
