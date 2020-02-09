//
//  small pingpong game that uses raylib
//  check https://www.raylib.com/cheatsheet/cheatsheet.html for API
//
//--------------------------------------------------------------------------------------

#include "raylib.h"
#include "player.h"
#include "math.h"

int lerp(int a, int b, float d) {
    return roundf(a * (1 - d) + b * d);
}

int main(void)
{
    // Initialization
    //--------------------------------------------------------------------------------------
    const int screenWidth = 800;
    const int screenHeight = 450;

    InitWindow(screenWidth, screenHeight, "pingpong raylib");

    Rectangle playerRect = { 400, 280, 40, 40 };
    player_t player = { 0, 0 };

    Camera2D camera = { 0 };
    camera.rotation = 0.0f;
    camera.zoom = 1.0f;
    camera.target = (Vector2){ screenWidth / 2, screenHeight / 2 };

    SetTargetFPS(60);

    // Main game loop
    while (!WindowShouldClose())        // Detect window close button or ESC key
    {
        // Input / Update
        //----------------------------------------------------------------------------------
        if (IsKeyPressed(KEY_RIGHT))
        {
            player.x++;
        }
        else if (IsKeyPressed(KEY_LEFT))
        {
            player.x--;
        }
        if (IsKeyPressed(KEY_UP))
        {
            player.y--;
        }
        else if (IsKeyPressed(KEY_DOWN))
        {
            player.y++;
        }
        
        // update the position
        playerRect.x = player.x * playerRect.width;
        playerRect.y = player.y * playerRect.height;
        camera.offset = (Vector2){
            lerp(camera.offset.x, (screenWidth - playerRect.width) / 2 - playerRect.x, 0.1),
            lerp(camera.offset.y, (screenHeight - playerRect.height) / 2 - playerRect.y, 0.1) };

        // Draw
        //----------------------------------------------------------------------------------
        BeginDrawing();

            ClearBackground(RAYWHITE);

            BeginMode2D(camera);
            
                // draw a grid
                for (int x = -10; x <= 10; x++)
                {
                    for (int y = -10; y <= 10; y++)
                    {
                        int xw = x * playerRect.width;
                        int yh = y * playerRect.height;
                        DrawLine(xw, -1000, xw, 1000, GREEN);
                        DrawLine(-1000, yh, 1000, yh, GREEN);
                    }
                }
                
                // draw the player
                DrawRectangleRec(playerRect, RED);

            EndMode2D();

            // show some UI text
            DrawRectangle( 10, 10, 250, 60, Fade(SKYBLUE, 0.5f));
            DrawRectangleLines( 10, 10, 250, 60, BLUE);
            DrawText("First test for a grid based game", 20, 20, 10, BLACK);
            DrawText("- Move with cursor keys", 40, 40, 10, DARKGRAY);

        EndDrawing();
        //----------------------------------------------------------------------------------
    }

    // De-Initialization
    //--------------------------------------------------------------------------------------
    CloseWindow();        // Close window and OpenGL context
    //--------------------------------------------------------------------------------------

    return 0;
}
