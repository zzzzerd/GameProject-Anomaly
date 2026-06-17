# Game Testing & Bug Fix Documentation

## Project: Anomaly
## Author: [Your Name]
## Date: [Presentation Date]

---

## 1. Player Movement Bugs

### Bug #1: Player Input Leak Warning
**Symptom:**
```
This will cause a leak and performance issues, PlayerInputControl.GamePlay.Disable() has not been called.
```

**Root Cause:**
- `inputControl.Enable()` was called in `Awake()` instead of `OnEnable()`
- `inputControl.Disable()` was called in `OnDisable()`
- Asymmetric enable/disable caused memory leak

**Fix:**
```csharp
// Before (WRONG)
private void Awake()
{
    inputControl.Enable();  // ❌ Called in Awake
}

private void OnDisable()
{
    inputControl.Disable();  // ❌ Not symmetric
}

// After (CORRECT)
private void OnEnable()
{
    inputControl.Enable();  // ✅ Symmetric with OnDisable
}

private void OnDisable()
{
    inputControl.Disable();  // ✅ Proper cleanup
}
```

**Lesson Learned:**
Always pair `Enable()` in `OnEnable()` with `Disable()` in `OnDisable()` for Input System.

---

### Bug #2: Player Moves Too Fast While Crouching
**Symptom:**
Player moves at normal speed while crouching, breaking gameplay balance.

**Root Cause:**
No speed adjustment when `isCrouch` is true.

**Fix:**
```csharp
public void Move()
{
    if (!wallJump)
    {
        float currentSpeed = isCrouch ? speed * 0.5f : speed;  // ✅ Reduce speed when crouching
        rb.velocity = new Vector2(inputDirection.x * currentSpeed * Time.deltaTime, rb.velocity.y);
    }
}
```

---

## 2. Enemy Detection & Movement Bugs

### Bug #3: Skeleton Not Chasing Player
**Symptom:**
Skeleton enemies stand still and don't chase the player.

**Root Cause:**
`checkSize` (detection box size) was set to `(0, 0)` in Inspector.

**Fix:**
- Set `checkSize` to `(1.5, 1.5)` in Unity Inspector
- Added debug logging to verify detection

**Debug Code Used:**
```csharp
Debug.Log($"[FoundPlayer] {gameObject.name} | 大小: {checkSize} | 命中: {hit}");
```

**Lesson Learned:**
Always verify Inspector values when detection fails. Use `OnDrawGizmosSelected()` to visualize detection zones.

---

### Bug #4: Boss Enemy Not Moving
**Symptom:**
Boss detects player (logs show detection) but doesn't move.

**Root Cause:**
Under investigation - `BossChaseState.PhysicsUpdate()` conditions may be blocking movement.

**Debug Steps Taken:**
1. Verified `FoundPlayer()` returns true ✅
2. Added logging to `PhysicsUpdate()` to check:
   - `isHurt`, `isDead`, `isAttack` flags
   - `moveDir` and `currentSpeed` values
   - `rb.velocity` assignment

**Status:** Pending - awaiting debug log output

---

## 3. Save/Load System Bugs

### Bug #5: Scene Objects Not Saving State
**Symptom:**
Activated statues/campfires reset to inactive state after scene reload.

**Root Cause:**
Objects not registering to `GameDataManager.savableList`.

**Fix:**
Implemented `ISaveService` interface with auto-registration:
```csharp
private void OnEnable()
{
    ISaveService saveble = this;
    saveble.TurnToSaveble();  // Auto-register
}

private void OnDisable()
{
    ISaveService saveble = this;
    saveble.TurnToUnsaveble();  // Auto-unregister
}
```

---

### Bug #6: Player Position Not Restored After Load
**Symptom:**
Player spawns at wrong position after loading save.

**Root Cause:**
`Character.LoadData()` not called, or `GameData.characterData` missing player ID.

**Fix:**
- Ensure `Character` implements `ISaveService`
- Verify `UniqueId` component is attached to player
- Check `GameDataManager.Load()` iterates through all savable objects

---

## 4. Animation & State Machine Bugs

### Bug #7: Hurt Animation Not Playing
**Symptom:**
Enemy doesn't play hurt animation when damaged.

**Root Cause:**
- `isHurt` flag not set in `OnTakeDamage()`
- Animation trigger not called

**Fix:**
```csharp
private void OnTakeDamage(Transform attacker)
{
    if (isHurt || isDead) return;
    
    isHurt = true;  // ✅ Set flag
    anim.SetTrigger("hurt");  // ✅ Trigger animation
    
    StartCoroutine(HurtRecovery());
}
```

---

## 5. Scene Management Bugs

### Bug #8: Game Over Panel Shows in Main Menu
**Symptom:**
Game over UI appears in main menu scene.

**Root Cause:**
UI elements not disabled when loading menu scene.

**Fix:**
```csharp
private void OnLoadEvent(GameSceneSO scene, Vector3 position, bool isLoading)
{
    bool isMenu = scene.sceneType == SceneType.Menu;
    playerStatBar.SetActive(!isMenu);  // ✅ Hide in menu
    gameOverPanel.SetActive(false);
}
```

---

## 6. Testing Checklist

### Before Each Build:
- [ ] All enemies detect player (check `checkSize` > 0)
- [ ] Player can save/load progress
- [ ] Scene transitions work without errors
- [ ] No input leak warnings in Console
- [ ] UI panels show/hide correctly per scene
- [ ] Audio plays correctly (BGM, SFX)
- [ ] Animations trigger properly (walk, hurt, die, attack)

### Debug Tools Used:
- `Debug.Log()` for state tracking
- `OnDrawGizmosSelected()` for visualization
- `[SerializeField]` for Inspector debugging
- Console filtering by tag (e.g., `[FoundPlayer]`)

---

## 7. Key Lessons Learned

### Architecture:
1. **Event-driven design prevents tight coupling** - Use `VoidEventSO` for cross-system communication
2. **Interface-based systems enable extensibility** - `ISaveService` allows any object to be saveable
3. **Persistent Scene simplifies manager lifecycle** - One place for UI, Audio, Data managers

### Unity-Specific:
1. **Always pair OnEnable/OnDisable** - Especially for Input System and events
2. **Verify Inspector values early** - Many bugs stem from unset public fields
3. **Use RequireComponent** - Ensures dependencies exist on GameObject

### Debugging Strategy:
1. **Reproduce consistently** - Find exact steps to trigger bug
2. **Isolate the system** - Comment out unrelated code
3. **Add logging strategically** - Check state at key points
4. **Visualize when possible** - Gizmos, Debug.DrawRay

---

## 8. Known Issues (If Any)

1. Boss movement investigation in progress
2. [Add any other known issues here]

---

## Notes for Presentation

**When asked about debugging:**
> "I used a systematic approach: first reproduce the bug, then isolate the system, add logging to track state, and fix the root cause rather than symptoms. I also learned to use Unity's visualization tools like Gizmos to debug detection zones visually."

**When asked about challenges:**
> "The biggest challenge was the save system. I had to design an architecture where any object could register itself without the manager knowing about it beforehand. That's why I used interfaces and auto-registration in OnEnable."

---

## Appendix: Useful Debug Code Snippets

### Visualize Detection Zone:
```csharp
public override void OnDrawGizmosSelected()
{
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(transform.position, checkDistance);
}
```

### Log Layer Mask:
```csharp
string layerName = LayerMask.LayerToName(Mathf.RoundToInt(Mathf.Log(attackLayer.value, 2)));
Debug.Log($"Detecting layer: {layerName}");
```

### Check Component Dependencies:
```csharp
[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(PhysicsCheck))]
public class Enemy : MonoBehaviour { }
```
