from flask import Flask, request
import serial
app = Flask(__name__)

ser = serial.Serial()
ser.port = "/dev/cu.usbmodem143301"
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

@app.route('/command')
def recieve_command():
    command = request.args.get("cmd")
    ser.write(command.encode())
    return command.encode()

if __name__ == "__main__":
    app.run(debug=True)