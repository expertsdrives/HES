// Scripts/bluetooth.js

async function connectBluetooth() {
    try {
        const device = await navigator.bluetooth.requestDevice({
            acceptAllDevices: true,
            optionalServices: ['battery_service']
        });

        const server = await device.gatt.connect();
        const service = await server.getPrimaryService('battery_service');
        const characteristic = await service.getCharacteristic('battery_level');
        const value = await characteristic.readValue();
        const batteryLevel = value.getUint8(0);

        alert(`Battery level is ${batteryLevel}%`);
    } catch (error) {
        console.error('Error connecting to Bluetooth device:', error);
    }
}
