public class BearEnragedState : IBearState
{
    private BearFSM _fsm;

    public BearEnragedState(BearFSM fsm)
    {
        _fsm = fsm;
    }

    public void Enter()
    {
        if (_fsm.IsEnraged) return; // 중복 진입 방지

        _fsm.SetEnraged(true); // 분노 활성화
        
        _fsm.RequestStateChange(EBearState.Chase);
    }

    public void Exit() { }

    public void Update() { }
}
