using Dalamud.Hooking;
using System;
using FFXIVClientStructs.FFXIV.Client.Game.Object;

namespace SamplePlugin;

// declare delegate


public unsafe class MyHook : IDisposable
{
    private delegate void OnLoadingHitbox(GameObject* gameObject, float unkownVal);
    private Hook<OnLoadingHitbox>? _onHitboxLoadedHook;

    public static void Initialize() { Instance = new MyHook(); }

    public static MyHook Instance { get; private set; } = null!;
    private MyHook()
    {
        try
        {
            _onHitboxLoadedHook = Plugin.GameInteropProvider.HookFromSignature<OnLoadingHitbox>(
                    "48 89 5C 24 08 57 48 83 EC 20 48 8B D9 E8 1E 1E FC FF",
                    OnHitboxLoaded
                );

            // Nullable because this might not have been initialized from IFA above, e.g. the sig was invalid.
            _onHitboxLoadedHook?.Enable();
        }
        catch (Exception e)
        {
            Plugin.Logger.Error("Error initiating hooks: " + e.Message);
        }

    }


    ~MyHook()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        _onHitboxLoadedHook?.Disable();
        _onHitboxLoadedHook?.Dispose();

    }

    private unsafe void OnHitboxLoaded(GameObject* gameObject, float unkownVal)
    {
        //try
        //{
        //    Plugin.Logger.Information("Hooked Here!");
        //}
        //catch (Exception ex)
        //{
        //    Plugin.Logger.Error(ex, "An error occured when handling hook.");
        //}

        // We're intentionally suppressing nullability checks. You can only get to this code if the hook exists.
        // There's no way this can ever be null.
        this._onHitboxLoadedHook?.Original(gameObject, unkownVal);

        try
        {
            //float hitboxRadius = gameObject->HitboxRadius;
            //string result = $"The value is {hitboxRadius:F2}";
            //Plugin.Logger.Information(result);
            gameObject->HitboxRadius += 3.0f;
        }
        catch (Exception ex)
        {
            Plugin.Logger.Error(ex, "An error occured when editing hitbox.");
        }
    }
}