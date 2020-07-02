import serial

ser = serial.Serial()
ser.port = "/dev/cu.usbmodem144101"
ser.baudrate = 9600
ser.setDTR(False)
ser.open()

while True:
    user_input = input().encode('utf-8')
    print(user_input)
    ser.write(bytes(user_input))