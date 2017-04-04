//Korean===================================================================================
[ 제작자 정보 ]
제작 : 이상원
e-mail : lswsox@gmail.com

[ 기능 ]
- 나선 궤적으로 움직입니다
- 다양한 랜덤 값을 지원합니다
- Ease-in 과 Ease-out 보간 방식을 선택할 수 있습니다
- 오브젝트에 자식으로 연결하면 오브젝트의 이동이나 회전을 반영할 수 있습니다
- 생성된 trail 들은 수명이 다하면 씬에서 자동 제거됩니다
- 불규칙적으로 흔들리는 Ziggle Move 특성을 선택할 수 있습니다.

[ 사용시 주의사항 ]
이 스크립트는 유니티 Trail Renderer를 사용합니다.
Trail Renderer는 느린 속도의 물체에 적용할 경우 깜빡거리는 Flickering 현상이 발생합니다.
Spiral Emitter 의 각종 수치를 조절하실 때 Trail이 적당히 빠른 속도를 유지할 수 있도록 해주세요
깜빡이는 Flickering 현상은 다음의 Min Vertex Distance 값을 줄여줌으로써 개선할 수 있습니다.

프리팹 SP-Move > SP-Rotate > SP-Distance 의 Min Vertex Distance 값은 씬의 스케일과 관련있는 값입니다.
이 값은 에디터에서 유저가 직접 값을 수정해야만 하는 값입니다.
이 값이 지나치게 작으면 성능에 악영향을 미칠 수 있으며 디폴트 값은 0.1 입니다.
이 값을 수정하는 방법 :
SP-Move 프리팹을 씬에 배치한 뒤 SP-Distance 노드의 Min Vertex Distance 값을 수정한 후 Prefab Apply

감사합니다.

//English===================================================================================
[ Developer ]
Name : Lee Sang-won
e-mail : lswsox@gmail.com

[ Features ]
- Spiral movement with TrailRenderer
- Many random properties
- Select Ease-in and Ease-out interpolation
- Child to object and Constrint to object (Move, Rotation)
- Auto-destruct generated trails
- Ziggle Move

[ Attention ]
This script uses Trail Renderer of Unity 3D.
Trail Renderer have flickering problem when head objects move slow.
You should keep speed enough to avoid flickering problem when edit parameters of Spiral Emitter.
Flickering issues can be improved by reducing the next "Min Vertex Distance" value.

"Min Vertex Distance" value is related to the scale of the scene
(SP-Move > SP-Rotate > SP-Distance)
This is a value that must be modified directly in the editor by user
If this value is too small, there is a negative impact on performance
The default value is 0.1
To modify this value:
After placing the SP-Move prefab on the scene,
fix the "Min Vertex Distance" value of node SP-Distance,
Press Inspector > Prefab > Apply


Thank you.