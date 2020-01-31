import os
from picamera import PiCamera
from time import sleep

camera = PiCamera()

print("taking")
camera.start_preview()
sleep(5)
camera.capture("input.jpg")
camera.stop_preview()

print("recog")

# get c# result
stream = os.popen("dotnet run -mode test -input input.jpg")
output = stream.read()
if(output =="1\n"):
    os.system("python move.py")
else:
    print("not!")
