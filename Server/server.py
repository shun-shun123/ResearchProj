from flask import Flask, request
import serial
import sys
import time

app = Flask(__name__)

is_use_serial_port = True

saved_commands = []

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


@app.route("/command/save")
def save_command():
    command = request.args.get("cmd")
    saved_commands.append(command)
    return "saved".encode()

@app.route("/command/save/send")
def send_saved_commands():
    if len(saved_commands) == 0:
        return "empty".encode()
    else:
        for command in saved_commands:
            ser.write(command.encode())
            print(command)
            time.sleep(2.5)
        return "send".encode()


if __name__ == "__main__":
    app.run(debug=True, host='0.0.0.0')