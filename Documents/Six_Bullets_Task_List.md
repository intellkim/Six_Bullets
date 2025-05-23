
# 🎯 Six Bullets – 개발 완료를 위한 과제 목록

아래는 게임 "Six Bullets"의 완성을 위해 수행해야 할 과제를 **효율적인 순서대로 정리한 목록**입니다.

---

## 🔹 1. 씬 구성 및 파일 구조 정리
- [ ] 각 챕터별 씬(Scene) 생성 및 이름 정리 (`Chapter1_Shoot`, `Chapter2_Escape` 등)
- [ ] 씬 전환 흐름 정리 (씬별 순서 및 분기)
- [ ] 기본 UI/Manager 프리팹 공통화

## 🔹 2. 타일맵 및 배경 씬 배치
- [ ] Tilemap 기반 배경 제작 (플랫포머 구간/광장 구간 등)
- [ ] 각 챕터 씬에 타일셋 배치
- [ ] 충돌판정(Ground 태그), 점프 가능 영역 설정

## 🔹 3. 조작 및 연출 튜토리얼 (프롤로그)
- [ ] 이동/점프 튜토리얼 안내 UI
- [ ] 경찰서로 가는 길 씬 구성
- [ ] 조력자 첫 등장 → 씬 전환

## 🔹 4. 1장 선택지 시퀀스 구현
- [ ] 타겟 인형 or 허공 사격 선택 구현
- [ ] 선택에 따른 씬 분기 (`BadEnd`, `Chapter2`)
- [ ] 총알 UI 연동

## 🔹 5. 2장 전투/배신자/탈출 구현
- [ ] 배신자 대사 & 컷신 연출
- [ ] 전등/배신자/경찰 중 사격 선택 분기 구현
- [ ] 조력자 부상 컷신 + 탈출 시퀀스
- [ ] 도망 시 이동속도 제한 연출

## 🔹 6. 3장 추격전 및 감정선 구성
- [ ] 지하철 씬 구성 및 추격자 등장
- [ ] 추격자 총 선택지 구현 (사격/도주/항복)
- [ ] 추격자 대사/조력자 퇴장 연출

## 🔹 7. 4장 군중 속 음모자 찾기 시퀀스
- [ ] 탑다운 시점 구현 + 이동 조작 전환
- [ ] 군중 타일 배치 + 음모자 등장
- [ ] 허공 사격 vs 목표물 사격 분기
- [ ] 정체 공개 및 5장으로 이어지는 컷신

## 🔹 8. 5장 대화/심리 선택 분기 구성
- [ ] 대사 시퀀스 및 회상 연출
- [ ] 음모자 정체 + 선택 3분기 (음모자/목표물/아무도X)
- [ ] 선택에 따른 PlayerPrefs 저장

## 🔹 9. 6장 메타 연출 + 크레딧 처리
- [ ] 화면 암전 연출 + 텍스트 천천히 출력
- [ ] “장전 소리” 연출
- [ ] 조작 불가 상태 처리
- [ ] 엔딩 크레딧 UI 구성

## 🔹 10. 공통 시스템/연출 고도화
- [ ] 대사 출력 시스템 (타이핑 효과)
- [ ] Fade In/Out, 카메라 흔들림 등 연출 통합
- [ ] 사운드 믹싱 (총성, 배경음, 심장박동 등)
- [ ] 총알 사용 UI 애니메이션

## 🔹 11. 세이브/로드 및 엔딩 분기 구조 정리
- [ ] PlayerPrefs 또는 SaveData 구조 정리
- [ ] 분기 트래킹 및 회상용 데이터 저장

## 🔹 12. UI/아트/사운드 디테일 다듬기
- [ ] HUD UI 정리 (대사창, 총알 HUD, 선택지 UI)
- [ ] 픽셀 아트 보강 (배경/캐릭터/이펙트)
- [ ] BGM/SE 최종 배치

## 🔹 13. 테스트 및 디버깅
- [ ] 각 분기 루트 전부 점검
- [ ] 총알 수 일관성 검증
- [ ] 이동/점프/충돌 버그 확인
- [ ] 씬 전환 및 저장 오류 확인

## 🔹 14. 최종 빌드 및 배포
- [ ] WebGL 또는 PC 빌드
- [ ] 실행파일 정리 및 옵션 설정
- [ ] 포트폴리오용 영상 캡처/스크린샷 제작
