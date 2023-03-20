# Don-t-miss-the-beat.
슈퍼센트 공모전 목적 모바일 리듬 게임

- Bit Related
현재 플레이 중인 노래의 BPM에 맞춰 비트 오브젝트를 생성하고 이를 판정 오브젝트의 Collider에 Enter했을 때와 Exit했을 때 각각의 함수를 처리한다.

- Sound Related
게임 시작시 BGM을 판정 오브젝트의 Collider에 Enter했을 때 Play한다.
그외 효과음 등을 관리하며 모두 재생된 AudioSource는 제거한다.

- Bit_obj
Image의 Scale을 조정하는 RuntimeAnimatorController를 판정 오브젝트의 Collider에 Exit했을 때 Play함으로써 적절한 타이밍에 바운스 애니메이션이 출력되도록 했다.
