import random
import os
# import pyautogui

'''
to do
-----------

features:
--
casual and strategic mode?
sfx!
--

code
--
make monster, player, map into classes? first do another class project
figure out how to do 'global' is a better way
docstrings and tests
go over whole project for improvements


design:
--
design game screens (win, loss etc)
\n before every prompt, line of text touching top of box looks bad

find music for ?opening?, for gameplay, for ?deaths?
https://www.youtube.com/watch?v=7JGWYhELgv4
https://www.youtube.com/watch?v=AW1zrt_cyF8
wilderness music?
daggerfall ost
pygame resources for music
--


other:
--
sometimes when starting it doesn't start, or at least print
        and sometimes input doesn't work in intro or start screen
--
            
'''

# index    0       1       2       3       4       5       6       7       8        9       10       11
CELLS = ((1, 5), (2, 5), (3, 5), (4, 5), (5, 5), (6, 5), (7, 5), (8, 5), (9, 5), (10, 5), (11, 5), (12, 5),
         (1, 4), (2, 4), (3, 4), (4, 4), (5, 4), (6, 4), (7, 4), (8, 4), (9, 4), (10, 4), (11, 4), (12, 4),
         (1, 3), (2, 3), (3, 3), (4, 3), (5, 3), (6, 3), (7, 3), (8, 3), (9, 3), (10, 3), (11, 3), (12, 3),
         (1, 2), (2, 2), (3, 2), (4, 2), (5, 2), (6, 2), (7, 2), (8, 2), (9, 2), (10, 2), (11, 2), (12, 2),
         (1, 1), (2, 1), (3, 1), (4, 1), (5, 1), (6, 1), (7, 1), (8, 1), (9, 1), (10, 1), (11, 1), (12, 1))


def clear_screen():
    os.system("cls" if os.name == "nt" else "clear")
    # pyautogui.keyDown("shift")
    # pyautogui.press("]")
    # pyautogui.keyUp("shift")
    pass


# creates all the entities, making sure they aren't taking up the same cell
def create_entities():
    global player_start
    global player
    global monster_start
    global monster
    global door
    global treasure

    created_entities = {}

    # generate monster, 2 tiles wide
    monster = random.sample(CELLS, 1)[0]
    x, y = monster
    if x == 1:
        monster = [(x, y), (x + 1, y)]
    else:
        monster = [(x - 1, y), (x, y)]
    created_entities["monster"] = monster

    # generate door on the perimeter, but not in another entity
    door_cells = []
    for cell in CELLS:
        x, y = cell
        if x == 1 or x == 12:
            door_cells.append(cell)
        elif y == 1 or y == 5:
            door_cells.append(cell)

    door = random.sample(door_cells, 1)[0]
    temp_list = []
    for item in list(created_entities.values()):
        if isinstance(item, list):
            for cell in item:
                temp_list.append(cell)
        elif isinstance(item, tuple):
            temp_list.append(item)
    while door in temp_list:
        door = random.sample(door_cells, 1)[0]
    else:
        created_entities["door"] = door

    # generate treasures anywhere not in the perimeter or in another entity
    treasure_cells = set(CELLS) - set(door_cells)
    number_of_pieces_of_treasure = random.randint(4, 6)
    treasure = random.sample(treasure_cells, number_of_pieces_of_treasure)
    temp_list = []
    for item in list(created_entities.values()):
        if isinstance(item, list):
            for cell in item:
                temp_list.append(cell)
        elif isinstance(item, tuple):
            temp_list.append(item)
    for piece_of_loot in treasure:
        while piece_of_loot in temp_list:
            treasure = random.sample(treasure_cells, number_of_pieces_of_treasure)
    else:
        created_entities["treasure"] = treasure

    # generate player anywhere not in another entity
    player = random.sample(CELLS, 1)[0]
    temp_list = []
    for item in list(created_entities.values()):
        if isinstance(item, list):
            for cell in item:
                temp_list.append(cell)
        elif isinstance(item, tuple):
            temp_list.append(item)
    while player in temp_list:
        player = random.sample(CELLS, 1)[0]
    else:
        created_entities["player"] = player

    player_start = created_entities["player"]
    player = created_entities["player"]
    monster_start = created_entities["monster"]
    monster = created_entities["monster"]
    door = created_entities["door"]
    treasure = created_entities["treasure"]


def draw_map():  # function doesn't return anything, only prints
    global player
    global monster
    global door
    global moves_taken
    global CELLS
    global treasure_opened
    global door_found
    global treasure_found
    global treasure

    map = []
    for cell in CELLS:
        x, y = cell
        if x == 12:
            line_end = "|"
        elif 1 <= x < 12:
            line_end = ""
        else:
            line_end = "DEBUG: draw_map failed; x of cell out of range 1-12"

        contents = "DEBUG: draw_map failed; no contents for cell prepared" + str(cell)
        if cell == player:
            contents = "*"
        elif cell in monster:
            contents = "X"
        elif cell == door:
            if door_found == "yes":
                contents = "D"
            else:
                contents = "_"  # same as empty contents below
        elif cell in treasure_opened:
            contents = "u"
        elif cell in treasure_found:
            contents = "T"
        elif cell in moves_taken:
            contents = "o"
        else:
            contents = "_"  # used in treasure debug contents, door_found contents

        map.append("|{}{}".format(contents, line_end))

    print("")
    print("".join(map[0:12]))
    print("".join(map[12:24]))
    print("".join(map[24:36]))
    print("".join(map[36:48]))
    print("".join(map[48:60]))
    print("")


def valid_moves():  # returns list of moves that player can take
    global player
    x, y = player
    moves = ["NORTH", "EAST", "SOUTH", "WEST", "WAIT"]
    if x == 1:
        moves.remove("WEST")
    if x == 12:
        moves.remove("EAST")
    if y == 1:
        moves.remove("SOUTH")
    if y == 5:
        moves.remove("NORTH")

    return moves


def player_move(text_input):
    global player
    global monster
    global door
    global treasure
    global moves_taken
    global turns_taken
    global treasure_opened

    x, y = player
    # add the moves_taken.append here if you want to add the PREVIOUS place to the list, not the new players place

    if text_input == "WAIT" or text_input == " ":
        pass
    elif text_input == "NORTH" or text_input == "W":
        y += 1
    elif text_input == "SOUTH" or text_input == "S":
        y -= 1
    elif text_input == "EAST" or text_input == "D":
        x += 1
    elif text_input == "WEST" or text_input == "A":
        x -= 1
    else:
        print("DEBUG: player_move function failed, text_input was not a readable input")
    player = x, y
    if player not in moves_taken:
        moves_taken.append(player)
    turns_taken += 1

    return player


def more_or_some():
    global treasure_carrying

    if len(treasure_carrying) > 0:
        return "more TREASURE"
    elif len(treasure_carrying) == 0:
        return "a piece of TREASURE"
    else:
        return "DEBUG: function more_or_some failed"


def check_player_for_interactions():  # see what happens now that the player is on a new tile, and do it
    global player
    global door
    global treasure
    global monster
    global door_found
    global trigger_door
    global treasure_found
    global hungry_turns
    global cell_of_sound
    global treasure_carrying
    global treasure_returned

    if player in monster:
        clear_screen()
        input("\n ** You have been eaten! A horrible death really. **\n")
        clear_screen()
        if len(treasure_opened) == 1:
            print("At least you grabbed a piece of loot for the monster to chew on.")
        elif len(treasure_opened) > 1:
            print("At least you grabbed {} pieces of loot for the monster to chew on".format(len(treasure_carrying)))
        draw_map()
        input("May the next adventurer who enters this dungeon find your map")
        return "player died"
    if player == door:
        if trigger_door == "yes":
            trigger_door = "no"
            if door_found == "no":
                input("\n ** You found the wooden door that leads outside! ** ")
                door_found = "yes"

            if len(treasure) == 0:
                leave_input = input("** Would you like to leave now and return your bulging pack to town? "
                                    "**\n Type LEAVE to escape.\n ").upper()
            else:
                leave_input = input("** Would you like to leave now, or would you like to risk staying to collect {}? "
                                    "**\n Type LEAVE to escape.\n ".format(more_or_some())).upper()
            if leave_input == "LEAVE":
                clear_screen()
                if len(treasure_opened) == 1:
                    for item in treasure_carrying:
                        treasure_returned.append(item)
                    input("\n ** Congratulations! You escaped with a piece of shiny loot! **\n")
                elif len(treasure_carrying) == (len(treasure) + len(treasure_carrying)):
                    input("\n ** Congratulations! You escaped with all of the amazing loot! "
                          "You are to be the talk of the town. "
                          "Pat yourself on the back for an amazing spelunk! **\n")
                elif len(treasure_carrying) > 1:
                    input("\n ** Congratulations! You escaped with {} pieces of shiny loot! **\n".format(
                        len(treasure_carrying)))
                else:
                    input("\n ** Congratulations! You escaped with your life. Spelunking is dangerous, "
                          "maybe it's time to go back to selling apples. **\n")

                return "player won"

            else:
                return "nothing special"

    if player in treasure:
        input("\n ** You found some amazing treasure and stuffed it in your backpack! ** \n")
        if len(treasure_opened) == 0:
            input(
                "\n ** In your haste to stuff your newfound golden chalices and crowns in your backpack, "
                "you never realized just how loud you were. Hopefully that's of no issue. ** \n")
        elif len(treasure_opened) > 0:
            input(
                "\n ** Even though you tried to be a little quieter this time, a backpack full of gold "
                "really is not silent. ** \n")
        if player not in treasure_opened:
            treasure_opened.append(player)
        treasure_found.append(player)
        treasure_carrying.append(player)
        treasure.remove(player)
        hungry_turns += 2
        cell_of_sound = player
        return "nothing special"
    else:
        return "nothing special"


def get_monster_valid_moves(monster_list_of_tuples):
    x1, y = monster_list_of_tuples[0]
    x2, y = monster_list_of_tuples[1]
    moves_for_monster = ["NORTH", "EAST", "SOUTH", "WEST"]
    if x1 <= 1 or x2 <= 1:
        moves_for_monster.remove("WEST")
    if x1 >= 12 or x2 >= 12:
        moves_for_monster.remove("EAST")
    if y <= 1:
        moves_for_monster.remove("SOUTH")
    if y >= 5:
        moves_for_monster.remove("NORTH")

    return moves_for_monster


def move_monster(monster_move):  # give an int to move that many times, else a "CARDINAL DIRECTION"
    global monster
    global cell_of_sound
    global hungry_turns
    global turns_taken
    global player
    global chase_turns

    x1, y = monster[0]
    x2, y = monster[1]
    if cell_of_sound:
        xs, ys = cell_of_sound
        if monster_move == "hungry":
            if x1 > xs:
                x1 -= 1
                x2 -= 1
            elif x2 < xs:
                x1 += 1
                x2 += 1
            elif y < ys:
                y += 1
            elif y > ys:
                y -= 1

    if isinstance(monster_move, int):
        tiles_monster_traversed_this_turn = [[(x1, y), (x2, y)]]
        valid_monster_moves = get_monster_valid_moves([(x1, y), (x2, y)])
        north = "possible"
        south = "possible"
        east = "possible"
        west = "possible"
        while monster_move:
            monster_move_direction = random.choice(valid_monster_moves)
            if north == "failed" and south == "failed" and east == "failed" and west == "failed":
                del monster_move
                break

            if monster_move_direction == "NORTH":
                if [(x1, y + 1), (x2, y + 1)] in tiles_monster_traversed_this_turn:
                    north = "failed"
                    pass
                else:
                    y += 1
                    monster_move -= 1
                    try:
                        valid_monster_moves = get_monster_valid_moves([(x1, y), (x2, y)])
                        valid_monster_moves.remove("SOUTH")
                    except ValueError:
                        pass
            elif monster_move_direction == "SOUTH":
                if [(x1, y - 1), (x2, y - 1)] in tiles_monster_traversed_this_turn:
                    south = "failed"
                    pass
                else:
                    y -= 1
                    try:
                        valid_monster_moves = get_monster_valid_moves([(x1, y), (x2, y)])
                        valid_monster_moves.remove("NORTH")
                    except ValueError:
                        pass
                    monster_move -= 1
            elif monster_move_direction == "WEST":
                if [(x1 - 1, y), (x2 - 1, y)] in tiles_monster_traversed_this_turn:
                    west = "failed"
                    pass
                else:
                    x1 -= 1
                    x2 -= 1
                    try:
                        valid_monster_moves = get_monster_valid_moves([(x1, y), (x2, y)])
                        valid_monster_moves.remove("EAST")
                    except ValueError:
                        pass
                    monster_move -= 1
            elif monster_move_direction == "EAST":
                if [(x1 + 1, y), (x2 + 1, y)] in tiles_monster_traversed_this_turn:
                    east = "failed"
                    pass
                else:
                    x1 += 1
                    x2 += 1
                    try:
                        valid_monster_moves = get_monster_valid_moves([(x1, y), (x2, y)])
                        valid_monster_moves.remove("WEST")
                    except ValueError:
                        pass
                    monster_move -= 1
        else:
            pass

    elif monster_move == "NORTH":
        y += 1
    elif monster_move == "SOUTH":
        y -= 1
    elif monster_move == "EAST":
        x1 += 1
        x2 += 1
    elif monster_move == "WEST":
        x1 -= 1
        x2 -= 1
    elif monster_move == "chase":
        while chase_turns:
            x1, y = monster[0]
            x2, y = monster[1]
            xp, yp = player
            if x1 > xp:
                x1 -= 1
                x2 -= 1
            elif x2 < xp:
                x1 += 1
                x2 += 1
            elif y < yp:
                y += 1
            elif y > yp:
                y -= 1

            chase_turns -= 1
    else:
        pass

    return [(x1, y), (x2, y)]


def monster_turn():  # number of turns taken "hungry" is in check_player_interactions
    global monster
    global turns_taken
    global hungry_turns
    global chase_turns

    if turns_taken == 4:  # will move after players fourth move
        input("\n ** You hear a low growl from somewhere inside the dark room - the monster has awakened! **\n")
        monster_move_direction = random.sample(get_monster_valid_moves(monster), 1)[0]
        monster = move_monster(monster_move_direction)

    elif hungry_turns and 4 < turns_taken < 12:  # update other turn checks
        input("\n ** You can hear the monster moving in the distance. ** \n")
        hungry_monster_moves = 2
        while hungry_monster_moves:
            monster = move_monster("hungry")
            hungry_monster_moves -= 1
        hungry_turns -= 1

    elif 4 < turns_taken < 12:
        d6 = random.randint(1, 6)
        if d6 <= 3:
            pass
        elif d6 == 4:
            input("\n ** You can hear the monster moving in the distance. ** \n")
            monster = move_monster(1)
        elif d6 == 5:
            input("\n ** You can hear the monster moving in the distance. ** \n")
            chase_turns = 1
            monster = move_monster("chase")
        elif d6 == 6:
            input("\n ** You hear growls and scraping claws on the walls around you - the monster is moving quickly! "
                  "Must be hungry. **\n")
            monster = move_monster(3)

    elif turns_taken == 12:
        input("\n ** You feel an eerie calm in the dungeon. You feel watched. ** \n")

    elif hungry_turns and turns_taken > 12:  # update other turn checks
        input("\n ** You can hear the monster moving in the distance. ** \n")
        hungry_monster_moves = 2
        while hungry_monster_moves:
            chase_turns = 1  # chase_turns per hungry_turn
            monster = move_monster("hungry")
            hungry_monster_moves -= 1
        hungry_turns -= 1

    elif turns_taken > 12:
        d6 = random.randint(1, 6)
        if d6 == 1:
            pass
        elif d6 == 2 or d6 == 3:
            input("\n ** You can hear the monster moving in the distance. ** \n")
            monster = move_monster(2)
        elif d6 == 4:
            input("\n ** You can hear the monster moving in the distance. ** \n")
            chase_turns = 2
            monster = move_monster("chase")
        elif d6 == 5 or d6 == 6:
            input(
                "\n ** You hear growls and scraping claws on the walls around you - the monster is moving quickly! "
                "Must be hungry. **\n")
            monster = move_monster(4)

    return {"monster": monster, "player check": check_player_for_interactions()}


def examine_input(player_input):
    if player_input == "QUIT":
        check = input("\nAre you sure you would like to exit?"
                      "\nType QUIT to leave completely.\n > ").upper()
        if check == "QUIT":
            return "input is quit"
        else:
            return "staying"
    legal_moves = valid_moves()
    if "NORTH" in legal_moves:
        legal_moves.append("W")
    if "EAST" in legal_moves:
        legal_moves.append("D")
    if "SOUTH" in legal_moves:
        legal_moves.append("S")
    if "WEST" in legal_moves:
        legal_moves.append("A")
    if "WAIT" in legal_moves:
        legal_moves.append(" ")
    if player_input in legal_moves:
        return player_input
    else:
        return "improper input"


def nicely_print_available_moves():
    available_moves = valid_moves()
    number_of_directions = 0
    if len(available_moves) > 2:
        for direction in available_moves[:-1]:
            available_moves[number_of_directions] = str(direction + ",")
            number_of_directions += 1
    if len(available_moves) > 1:
        available_moves.insert(-1, "or")
    print("You can move {}".format(" ".join(available_moves)))


def game_loop():
    global player
    global door
    global monster
    global treasure
    global treasure_opened
    global turns_taken
    global player_start
    global trigger_door
    global treasure_carrying

    while True:
        # everything below this comment goes into "elif examine.."
        trigger_door = "yes"
        clear_screen()
        print("You have taken {} turns".format(turns_taken))
        if len(treasure_carrying) == 1:
            print("You are carrying a piece of treasure")
        elif len(treasure_carrying) > 1:
            print("You are carrying {} pieces of treasure".format(len(treasure_carrying)))
        draw_map()
        nicely_print_available_moves()
        # everything above this comment goes into "elif examine... == improper input
        move_input = input("> ").upper()
        examined_output = examine_input(move_input)
        if examined_output == "improper input":
            input("** Move was invalid **\n Press enter to continue")
            continue
        elif examined_output == "input is quit":
            return "player is quitting"
        elif examined_output == "staying":
            continue
        else:
            player = player_move(examined_output)
            player_happenings = check_player_for_interactions()
            if player_happenings == "nothing special":
                pass
            else:
                return player_happenings

        monster_happenings = monster_turn()
        monster = monster_happenings["monster"]
        if monster_happenings["player check"] == "nothing special":
            pass
        else:
            return monster_happenings["player check"]


def check_skip():
    reaction = input("").upper()
    if reaction == "START":
        return "skip"
    else:
        pass


def play_game(state):
    global player
    global monster
    global door
    global treasure
    global moves_taken
    global turns_taken
    global treasure_opened
    global door_found
    global trigger_door
    global treasure_found
    global player_start
    global monster_start
    global hungry_turns
    global cell_of_sound
    global treasure_carrying
    global treasure_returned
    global intro
    global intro_returning

    while intro:
        clear_screen()
        print("\n** Two weeks ago you heard of a great hoard of treasure in a nearby cavern, "
              "and set off to claim it. ** ")
        if check_skip() == "skip":
            intro = False
            break

        clear_screen()
        print("\n ** You found the cavern after quite the search. You approached it cautiously. ** ")
        if check_skip() == "skip":
            intro = False
            break

        clear_screen()
        print("\n ** There is told to be a monster inside. "
              "You camped till dusk. You wanted to plunder while the monster was sleeping. ** ")
        if check_skip() == "skip":
            intro = False
            break

        clear_screen()
        print("\n ** As the tales go, fortunately that monster is blind. Unfortunately that probably means "
              "acute hearing. You are careful to quietly open the creaky wooden door to the cavern. ** ")
        if check_skip() == "skip":
            intro = False
            break

        clear_screen()
        print("\n ** You didn't get far into the cavern before your torch went out from a "
              "sudden gust of cold air. ** ")
        if check_skip() == "skip":
            intro = False
            break

        clear_screen()
        print("\n ** You relight a new torch after some fumbling through your large pack, "
              "and look up to realize you are lost. There is no light save your own torch, "
              "and you can barely see your own hands. ** ")
        if check_skip() == "skip":
            intro = False
            break

        clear_screen()
        print("\n ** You have heard no stirrings from the monster. You see it sleeping on the floor. ** ")
        if check_skip() == "skip":
            intro = False
            break

        clear_screen()
        print("\n ** Good luck. ** ")
        if check_skip() == "skip":
            intro = False
            break

        intro = False

    while intro_returning:
        clear_screen()
        print("\n** As a young one you grew up on the adventurous tales the people of your town spun. ** ")
        if check_skip() == "skip":
            intro_returning = False
            break

        clear_screen()
        print("\n ** The story that always stuck with you was of the ambitious soul who travelled south "
              "to the caves and never returned. ** ")
        if check_skip() == "skip":
            intro_returning = False
            break

        clear_screen()
        print("\n ** It is told they were young and didn't have many friends, "
              "and so they ventured out for fame as much as wealth. ** ")
        if check_skip() == "skip":
            intro_returning = False
            break

        clear_screen()
        print("\n ** This story always stuck with you because you thought it was a great idea! **\n "
              "** You planned the second you celebrate your lonely 16th birthday you'd follow their footsteps, "
              "and hopefully return with glory to match your ambition. ** ")
        if check_skip() == "skip":
            intro_returning = False
            break

        clear_screen()
        print("\n ** You reached the cavern without hitch, and planned to enter when it was dusk. "
              "The blind monster should be asleep then. ** ")
        if check_skip() == "skip":
            intro_returning = False
            break

        clear_screen()
        print("\n ** You didn't get far into the cavern before your torch went out from a "
              "sudden gust of cold air. ** ")
        if check_skip() == "skip":
            intro_returning = False
            break

        clear_screen()
        print("\n ** As you relight your torch, you see a faded though fully readable map on the ground. **\n")
        draw_map()
        if check_skip() == "skip":
            intro_returning = False
            break

        clear_screen()
        print("\n ** Good luck. ** ")
        if check_skip() == "skip":
            intro_returning = False
            break

        intro_returning = False

    if state == "playing":
        create_entities()
        hungry_turns = 0
        moves_taken = [player]
        turns_taken = 0
        treasure_opened = []
        treasure_found = []
        treasure_returned = []
        treasure_carrying = []
        door_found = "no"
        cell_of_sound = None
        trigger_door = "yes"
        # define all globals before this point

        game_loop_result = game_loop()
        if game_loop_result == "player is quitting":
            state = "quitting"
        elif game_loop_result == "player died":
            state = "dead"
        elif game_loop_result == "player won":
            state = "won"

    if state == "playing again":
        # don't reset: moves_taken = [player]
        moves_taken.append(player)
        hungry_turns = 0
        cell_of_sound = None
        turns_taken = 0
        for item in treasure_found:
            if item not in treasure:
                treasure.append(item)
        treasure_opened = []
        treasure_carrying = []
        player = player_start
        monster = monster_start
        # don't reset: treasure_found = []
        # don't reset: door_found = "no"
        trigger_door = "yes"
        # define all globals before this point

        game_loop_result = game_loop()
        if game_loop_result == "player is quitting":
            state = "quitting"
        elif game_loop_result == "player died":
            state = "dead"
        elif game_loop_result == "player won":
            state = "won"

    if state == "quitting":
        clear_screen()
        print("You quit - Thank you for playing!")
    if state == "dead":
        clear_screen()
        input("\n Your spelunk resulted in death! You survived {} turns.\n\n "
              "You are the talk of the town, some talk of your foolishness, some of your arrogance, "
              "and most of your passing. "
              "\n Your quest is an inspiration and a legend for the children of the town.\n"
              .format(turns_taken))
        clear_screen()
        play_again_input = input("\n Spelunk again?\nType QUIT to leave. \n").upper()
        if play_again_input == 'QUIT':  # this block is used again below in "won"
            intro = False
            play_game("quitting")
        else:
            state = "playing again"
            intro_returning = True
            play_game(state)
    if state == "won":
        clear_screen()
        input("\n Spelunk complete! You took {} turns!\n\n "
              "You are the talk of the town, some talk of your success, some of your lack there-of, "
              "and most about your determination.\n".format(turns_taken))
        clear_screen()
        play_again_input = input("\n Spelunk again?\n Type QUIT to leave. \n").upper()
        if play_again_input == 'QUIT':  # this block is used again above in "dead"
            intro = False
            play_game("quitting")
        else:
            intro = True
            play_game("playing")


def start_screen():
    global intro
    global intro_returning
    intro_returning = False

    print("")
    print(" ", "*" * 32)
    print(" " * 3, "*" * 28)
    print("\n", " " * 13, "DUNGEON")
    print("\n", " " * 3, "*" * 28)
    print(" " * 1, "*" * 33, "\n" * 3)
    print(" Type START to skip the intro.")
    print(" Type QUIT to exit at any time.")
    starting_input = input(" Press enter to begin!\n ").upper()
    if starting_input == "QUIT":
        return "quitting"
    elif starting_input == "START":
        intro = False
        return "playing"
    else:
        intro = True
        return "playing"


play_game(start_screen())
