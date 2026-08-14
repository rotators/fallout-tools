#ifndef SAY_HELLO_H
#define SAY_HELLO_H

#include "./hello_world_text.h"
#include <./hello_world_text.h>

procedure sayHello(variable desc) begin
    display_msg(HELLO_WORLD_TEXT + ": " + desc);
end

#endif


