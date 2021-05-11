export { map, loadMap, getLonLat }

const map = document.querySelector('#map')

function loadMap() {
	const topLeft = { lon: 10.2545, lat: 56.5498 }
	const bottomRight = { lon: 14.1957, lat: 57.8154 }
	const mapboxToken =
		'pk.eyJ1IjoiYmpvcm4tc3Ryb21iZXJnIiwiYSI6ImNrbmtiMDk5czA5Zm4ycHBtcGZtZWcxYnMifQ.cUxPUwyuobn9PMPo_8JnSw'
	const mapUrl =
		'https://api.mapbox.com/styles/v1/mapbox/streets-v11/static/' +
		`[${topLeft.lon},${topLeft.lat},${bottomRight.lon},${bottomRight.lat}]/` +
		'1080x640@2x' +
		`?access_token=${mapboxToken}`
	map.src = mapUrl
}

function getLonLat() {
	const topLeft = { lon: 10.2545, lat: 56.5498 }
	const bottomRight = { lon: 14.1957, lat: 57.8154 }
	const rect = map.getBoundingClientRect()
	const lon2px = rect.width / (bottomRight.lon - topLeft.lon)
	const lat2px = rect.height / (bottomRight.lat - topLeft.lat)
	const px2lon = 1.0 / lon2px
	const px2lat = 1.0 / lat2px
	return { lat: 0, lon: 0 }
}
