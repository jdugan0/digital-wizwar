UP = 0b1000
DOWN = 0b0100
LEFT = 0b0010
RIGHT = 0b0001
while(True):
    x = input("Input directions: ")
    x = x.lower()
    if (x == "end" or x == "exit" or x == "done" or x == "stop" or x == "quit"):
        break
    y = x.split(",")
    ints = 0b0000
    for z in y:
        if (z == "u"):
            ints |= UP
        if (z == "d"):
            ints |= DOWN
        if (z == "l"):
            ints |= LEFT
        if (z == "r"):
            ints |= RIGHT

    print(ints)