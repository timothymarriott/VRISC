
using Raylib_cs;

namespace VRISC.Emulator.GPU;

public enum DisplayMode
{
    Console = 0,
    BlackAndWhite = 1,
    Palette = 2,
    Bitmap = 3
}
    
public class GPU
{

    public DisplayMode CurrentDisplayMode;

    public VRam vram;


    public int width;

    public int height;

    public RenderTexture2D target;

    public bool blitRequested;
    
    
    private EmulatorState owner;
    public GPU(EmulatorState owner)
    {
        this.owner = owner;
        
    }


    public void Blit()
    {

        blitRequested = true;

    }

    public void Render()
    {
        if (target.Texture.Width != width || target.Texture.Height != height)
        {
            Raylib.UnloadRenderTexture(target);
            target = Raylib.LoadRenderTexture(width, height);
            
        }
        
        Raylib.BeginTextureMode(target);
        Raylib.ClearBackground(Color.Black);
        switch (CurrentDisplayMode)
        {
            case DisplayMode.BlackAndWhite:
                if (vram.GetSize() * 8 < width * height)
                {
                    Log.Error("VRam too small for frame buffer size.");
                }
                else
                {
                    
                    for (int i = 0; i < width * height; i++)
                    {
                        Raylib.DrawPixel(i % width, (int)Math.Floor((float)i / (float)width), new Color(vram.GetByte(i), vram.GetByte(i), vram.GetByte(i), (byte)255));
                    }
                }
                break;
            case DisplayMode.Bitmap:
                if (vram.GetSize() * 8 < width * height)
                {
                    Log.Error("VRam too small for frame buffer size.");
                }
                else
                {
                    
                    for (int i = 0; i < width * height; i+=3)
                    {
                        Raylib.DrawPixel((i/3) % width, (int)Math.Floor((float)(i/3) / (float)width), new Color(vram.GetByte(i), vram.GetByte(i + 1), vram.GetByte(i + 2), (byte)255));
                    }
                }
                break;
            
        }

        Raylib.EndTextureMode();
    }

    
}