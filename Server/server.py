from flask import Flask
import serial
app = Flask(__name__)

ser = serial.Serial()
ser.port = "/dev/cu.usbmodem144101"
ser.baudrate = 9600
ser.setDTR(False)
ser.open()

@app.route('/on')
def serial_on():
    ser.write(b'1')
    return "ON"

@app.route('/off')
def serial_off():
    ser.write(b'0')
    return "OFF"

if __name__ == "__main__":
    app.run(debug=True)