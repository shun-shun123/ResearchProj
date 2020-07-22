from flask import Flask, request
import serial
import sys

app = Flask(__name__)

is_use_serial_port = True

if len(sys.argv) > 1:
    is_use_serial_port = int(sys.argv[1]) == 0

if is_use_serial_port:
    ser = serial.Serial()
    ser.port = "/dev/cu.usbmodem143301"
    ser.baudrate = 9600
    ser.setDTR(False)
    ser.open()

@app.route('/on')
def serial_on():
    if is_use_serial_port:
        ser.write(b'1')
    return "ON"

@app.route('/off')
def serial_off():
    if is_use_serial_port:
        ser.write(b'0')
    return "OFF"

@app.route('/command')
def recieve_command():
    command = request.args.get("cmd")
    if is_use_serial_port:
        ser.write(command.encode())
    else:
        print(command)
    return command.encode()

if __name__ == "__main__":
    app.run(debug=True, host='0.0.0.0')