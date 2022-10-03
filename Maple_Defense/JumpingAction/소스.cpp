/*
�Ϲ� ���ð��Ӱ��� �ٸ��� �������� �������� �ϴ� ���ð����� ����� ���ҽ��ϴ�.
*/

#pragma comment(lib, "msimg32.lib")
#pragma comment(lib,"Project22.lib")

#include <windows.h>
#include <sql.h>
#include <math.h>
#include <sqlext.h>
#include <time.h>
#include "resource.h"

#define TrashColor RGB(255,0,255)

LRESULT CALLBACK WndProc(HWND, UINT, WPARAM, LPARAM);
HINSTANCE g_hInst;
HWND hWndMain;
LPCTSTR lpszClass = TEXT("Boss Shooting Game");

// ��ȭ���� Proc
BOOL CALLBACK RegisterProc(HWND hDlig, UINT iMessage, WPARAM wParam, LPARAM lParam);
BOOL CALLBACK LoginProc(HWND hDlig, UINT iMessage, WPARAM wParam, LPARAM lParam);

// �Լ�
void Init();
void GetKey(); // GetKeyState �޾ƿ� �Լ�
void Collision();
void Scroll(HDC hMemDC);
void Stage(RECT rt);
void Jump();

// DLL �Լ���
void DrawBitmap(HDC hdc, int x, int y, HBITMAP hBit);
void DrawScroll(HDC hdc, int x, int y, int x2, int y2, int x3, HBITMAP hBit);
void TransBlt(HDC hdc, int x, int y, HBITMAP hBit, COLORREF color);
void AnimBlt(HDC hdc, int x, int y, int wx, int wy, int sx, int sy, HBITMAP hBit, COLORREF color);

// ���� Ŭ����
//LRESULT CALLBACK EditSubProc(HWND hWnd, UINT iMessage, WPARAM wParam, LPARAM lParam);

int APIENTRY WinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance
	, LPSTR lpszCmdParam, int nCmdShow)
{
	HWND hWnd;
	MSG Message;
	WNDCLASS WndClass;
	g_hInst = hInstance;

	WndClass.cbClsExtra = 0;
	WndClass.cbWndExtra = 0;
	WndClass.hbrBackground = (HBRUSH)(COLOR_WINDOW + 1);
	WndClass.hCursor = LoadCursor(NULL, IDC_ARROW);
	WndClass.hIcon = LoadIcon(NULL, IDI_APPLICATION);
	WndClass.hInstance = hInstance;
	WndClass.lpfnWndProc = WndProc;
	WndClass.lpszClassName = lpszClass;
	WndClass.lpszMenuName = MAKEINTRESOURCE(IDR_MENU1);
	WndClass.style = CS_HREDRAW | CS_VREDRAW;
	RegisterClass(&WndClass);

	hWnd = CreateWindow(lpszClass, lpszClass, WS_OVERLAPPED | WS_SYSMENU,
		0, 0, 1080, 758,
		NULL, (HMENU)NULL, hInstance, NULL);
	ShowWindow(hWnd, nCmdShow);

	while (GetMessage(&Message, NULL, 0, 0)) {
		TranslateMessage(&Message);
		DispatchMessage(&Message);
	}
	return (int)Message.wParam;
}

#pragma region  ������

// �ڵ�
SQLHENV hEnv;
SQLHDBC hDbc;
SQLHSTMT hStmt;

// ������� �����ޱ� ���� ������
SQLCHAR P_NAME[50], P_ID[50], P_PASSWORD[50], P_EMAIL[50];
int P_Score;
SQLINTEGER lP_NAME, lP_ID, lP_PASSWORD, lP_EMAIL, lP_Score;

// ȭ�� ����� ���� ������
HBITMAP hBit, hBackBit, BackGround;
HDC hMemDC;
HDC hdc;
TCHAR str[255];
HMENU hMenu;

// ����
double jx = 10.f;
double jy = 500.f;
double diameter = 20.f;

float JumpTime = 0.f;
float JumpHeight = 0;
float JumpPower = 50.f;

BOOL g_bJumpkeyPressed = FALSE;

struct Current_Info // ���� ���� ���� ����ü 
{
	TCHAR Cur_ID[256];
	TCHAR Cur_Email[256];
	int Level = 1;
	int Level_Speed;
	int Max_Index;
	int Life;
	int cur_score = 10000;
	int goal_score;
	int player_select_key = -1;
	POINT pt;
	int checkCount = 0;
	bool checkOn = false;
	bool GameStart = true;
	bool GameOver = false;
	HBITMAP inGameBg;
	HBITMAP RankBg;
	HBITMAP Clear;
	HBITMAP StoreBg;
	HBITMAP SelectBg;
	HBITMAP Cur_Select;
};

struct PlayerBullet_Info
{
	HBITMAP PB_BITMAP;  // �Ѿ� ��Ʈ�� 
	bool isShot = false;// �Ѿ��� ���� �ƴ��� �Ǻ��� bool ����
	int PB_POS_x;// ��ǥ
	int PB_POS_y;// ��ǥ
	RECT bulletRect;//	�Ѿ��� RECT
};

struct Player_Info // �÷��̾� ����ü
{
	HBITMAP P_B;
	HBITMAP P_SLOT;
	HBITMAP P_LEVELUP;
	bool isLevelUp = false;
	int LevelUpIndex = 0;
	RECT P_RT;
	int px = 50; // ��ǥ
	int py = 550; // ��ǥ
	int level = 0;
	int speed = 0;
	int bulletIndex = 0;
	int damage = 0;
	int health = 100;
	int pashealth = 100;

	// �÷��̾� �ִϸ��̼� �ε���
	int WalkIndex = 1;
	int SkillIndex = 0;

	// �÷��̾� ��ȭ
	HBITMAP P_CHANGE;
	bool P_CHANGE_ON = false;
	int P_CHANGE_INDEX = 0;
	int P_CHANGE_ANI_INDEX = 0;

	// �÷��̾� ü��
	HBITMAP P_HP;
	HBITMAP P_HP_UI;

	// �÷��̾� ��ų��
	bool SkillOn = false;
	HBITMAP P_SKILL_1;
	bool SKILL_1 = false;
	HBITMAP P_SKILL_2;
	bool SKILL_2 = false;
	HBITMAP P_SKILL_3;
	HBITMAP P_SKILL_3_ICON;
	bool SKILL_3_ENABLE = false;
	bool SKILL_3 = false;
	HBITMAP P_SKILL_4;
	HBITMAP P_SKILL_4_ICON;
	bool SKILL_4_ENABLE = false;
	bool SKILL_4 = false;
	int P_SKILL_5_INDEX = 0;
	HBITMAP P_SKILL_5;
	HBITMAP P_SKILL_5_ICON;
	bool SKILL_5_ENABLE = false;
	bool SKILL_5 = false;
	HBITMAP P_SKILL_6;
	HBITMAP P_SKILL_6_ICON;
	bool SKILL_6_ENABLE = false;
	bool SKILL_6 = false;
	RECT P_SKILLRT;

	PlayerBullet_Info P_Bullet[50];
};

struct EnemyBullet_Info
{
	HBITMAP EB_BITMAP;  // �Ѿ� ��Ʈ�� 
	bool isShot = false;// �Ѿ��� ���� �ƴ��� �Ǻ��� bool ����
	int EB_POS_x;// ��ǥ
	int EB_POS_y;// ��ǥ
	RECT bulletRect;//	�Ѿ��� RECT
	int bulletIndex = 0;
};

struct Enemy_Info // 
{
	HBITMAP E_B;
	RECT E_RT;
	EnemyBullet_Info E_Bullet[50];
	bool isLive = false;
	int health;
	int px; // ��ǥ
	int py; // ��ǥ
	int walkIndex = 0;
};

struct Boss_Info
{
	HBITMAP B_B;
	HBITMAP B_HPB;
	HBITMAP B_SKILL_EFFECT;
	RECT B_RT;
	EnemyBullet_Info B_Bullet[3][50];
	bool isSkillC = false;
	bool isLive = true;
	bool isHit = false;
	bool isSkill = false;
	bool isDie = false;
	bool isSpawn = false;
	int SpawnIndex = 0;
	int bossForm = 1;
	int level = 1;
	int health;
	int pasHealth;
	int px; // ��ǥ
	int py; // ��ǥ
	int walkIndex = 0;
	int SkillIndex = 0;
	int DieIndex = 0;
	int SkillTimer = 0;
};

struct Rank_Info // ��ŷ ����ü
{
	TCHAR R_Name[256];
	int R_Score;
};

// ���� ���� ����� ����ü��
Current_Info Cur_Info;
Enemy_Info enemy_Info[9][10];
Boss_Info boss_Info;
Player_Info Player;
Rank_Info Ranking[256];

// ����
int RankIndex = 0;
int timerCount = 0;
int y = 0; // ��ũ�� �ϱ� ���� y��ǥ�� �޾��ִ� ����
int x = 0;
int phase = -1;

// ����Ŭ���̿� �ʿ��� ������
#define ID_EDIT1 101
HWND hEdit; 				// ���������� ����
WNDPROC OldEditProc;	// ���������� ����

// 
HANDLE hEvent1;

#pragma endregion // ������ �����س���. �������ϰ� region

BOOL DBConnect()
{
	// ���� ������ ���� ������
	SQLCHAR InCon[255];
	SQLCHAR OutCon[1024];
	SQLSMALLINT cbOutCon;
	TCHAR Dir[MAX_PATH];
	SQLRETURN Ret;

	// ȯ�� �ڵ��� �Ҵ��ϰ� ���� �Ӽ��� �����Ѵ�.
	if (SQLAllocHandle(SQL_HANDLE_ENV, SQL_NULL_HANDLE, &hEnv) != SQL_SUCCESS)
		return FALSE;
	if (SQLSetEnvAttr(hEnv, SQL_ATTR_ODBC_VERSION, (SQLPOINTER)SQL_OV_ODBC3,
		SQL_IS_INTEGER) != SQL_SUCCESS)
		return FALSE;

	// ���� �ڵ��� �Ҵ��ϰ� �����Ѵ�.
	if (SQLAllocHandle(SQL_HANDLE_DBC, hEnv, &hDbc) != SQL_SUCCESS)
		return FALSE;

	// MDB ���Ͽ� �����ϱ�
	GetCurrentDirectory(MAX_PATH, Dir);
	wsprintf((TCHAR*)InCon, "DRIVER={Microsoft Access Driver (*.mdb)};"
		"DBQ=%s\\Jumping.mdb;", Dir);
	Ret = SQLDriverConnect(hDbc, hWndMain, InCon, sizeof(InCon), OutCon, sizeof(OutCon),
		&cbOutCon, SQL_DRIVER_NOPROMPT);

	if ((Ret != SQL_SUCCESS) && (Ret != SQL_SUCCESS_WITH_INFO))
		return FALSE;

	// ��� �ڵ��� �Ҵ��Ѵ�.
	if (SQLAllocHandle(SQL_HANDLE_STMT, hDbc, &hStmt) != SQL_SUCCESS)
		return FALSE;

	return TRUE;
}

void DBDisConnect()
{
	// ������
	if (hStmt) SQLFreeHandle(SQL_HANDLE_STMT, hStmt);
	if (hDbc) SQLDisconnect(hDbc);
	if (hDbc) SQLFreeHandle(SQL_HANDLE_DBC, hDbc);
	if (hEnv) SQLFreeHandle(SQL_HANDLE_ENV, hEnv);
}

BOOL DBExecuteSQL()
{
	// ����� �����ޱ� ���� ���ε��Ѵ�.
	SQLBindCol(hStmt, 1, SQL_C_CHAR, P_NAME, sizeof(P_NAME), &lP_NAME);
	SQLBindCol(hStmt, 2, SQL_C_CHAR, P_ID, sizeof(P_ID), &lP_ID);
	SQLBindCol(hStmt, 3, SQL_C_CHAR, P_PASSWORD, sizeof(P_PASSWORD), &lP_PASSWORD);
	SQLBindCol(hStmt, 4, SQL_C_CHAR, P_EMAIL, sizeof(P_EMAIL), &lP_EMAIL);
	SQLBindCol(hStmt, 5, SQL_C_ULONG, &P_Score, 0, &lP_Score);

	// SQL���� �����Ѵ�.
	if (SQLExecDirect(hStmt, (SQLCHAR*)"select P_Name,P_ID,P_Password,P_Email,P_Score from LoginTbl",
		SQL_NTS) != SQL_SUCCESS) {
		return FALSE;
	}

	return TRUE;
}

DWORD WINAPI Thread1(LPVOID temp) // ������ 
{
	WaitForSingleObject(hEvent1, INFINITE); // hEvent�� ���� �� ���� ��ٸ�.

	HDC hdc = GetDC(hWndMain);
	RECT rt;
	GetClientRect(hWndMain, &rt);

	if (hBackBit == NULL) {
		hBackBit = CreateCompatibleBitmap(hdc, rt.right, rt.bottom);
	}

	Stage(rt);
	return 0;
}

// �׾��� �� ��ŷ ����� ������� ��� ���ư��� �Լ�
void OnTimer()
{
}

// ���� ���Ͻ�
LRESULT CALLBACK WndProc(HWND hWnd, UINT iMessage, WPARAM wParam, LPARAM lParam)
{
	TCHAR str[256];
	PAINTSTRUCT ps;
	HDC hdc;
	switch (iMessage) {
	case WM_CREATE:
		hWndMain = hWnd;
		hMenu = GetMenu(hWnd);
		Init(); // ���� �ʱ� ���� �Լ� ���� 
		SetEvent(hEvent1);
		return 0;
	case WM_TIMER:
		switch (wParam)
		{
		case 1:
			GetKey();
			break;
		case 2:
			if (Player.WalkIndex <= 2)
				Player.WalkIndex++;
			else
				Player.WalkIndex = 0;


			for (int k = 0; k < 5; k++)
			{
				// 1�� ����
				if (enemy_Info[0][k].isLive)
				{
					if (enemy_Info[0][k].walkIndex <= 3)
						enemy_Info[0][k].walkIndex++;
					else
						enemy_Info[0][k].walkIndex = 0;
				}
				// 2�� ����
				if (enemy_Info[1][k].isLive)
				{
					if (enemy_Info[1][k].walkIndex <= 2)
						enemy_Info[1][k].walkIndex++;
					else
						enemy_Info[1][k].walkIndex = 0;
				}
				// 3�� ����
				if (enemy_Info[2][k].isLive)
				{
					if (enemy_Info[2][k].walkIndex <= 2)
						enemy_Info[2][k].walkIndex++;
					else
						enemy_Info[2][k].walkIndex = 0;
				}
				// 4�� ����
				if (enemy_Info[3][k].isLive)
				{
					if (enemy_Info[3][k].walkIndex <= 2)
						enemy_Info[3][k].walkIndex++;
					else
						enemy_Info[3][k].walkIndex = 0;
				}
				// 5�� ����
				if (enemy_Info[4][k].isLive)
				{
					if (enemy_Info[4][k].walkIndex <= 4)
						enemy_Info[4][k].walkIndex++;
					else
						enemy_Info[4][k].walkIndex = 0;
				}
				// 6�� ����
				if (enemy_Info[5][k].isLive)
				{
					if (enemy_Info[5][k].walkIndex <= 1)
						enemy_Info[5][k].walkIndex++;
					else
						enemy_Info[5][k].walkIndex = 0;
				}
				// 7�� ����
				if (enemy_Info[6][k].isLive)
				{
					if (enemy_Info[6][k].walkIndex <= 4)
						enemy_Info[6][k].walkIndex++;
					else
						enemy_Info[6][k].walkIndex = 0;
				}
				// 8�� ����
				if (enemy_Info[7][k].isLive)
				{
					if (enemy_Info[7][k].walkIndex <= 4)
						enemy_Info[7][k].walkIndex++;
					else
						enemy_Info[7][k].walkIndex = 0;
				}
				// 9�� ����
				if (enemy_Info[8][k].isLive)
				{
					if (enemy_Info[8][k].walkIndex <= 2)
						enemy_Info[8][k].walkIndex++;
					else
						enemy_Info[8][k].walkIndex = 0;
				}
			}
#pragma region 1�� ����
			// 1�� ����
			if (phase == 1)
			{
				if (boss_Info.isLive && phase == 1)
				{
					if (boss_Info.walkIndex <= 4)
						boss_Info.walkIndex++;
					else
						boss_Info.walkIndex = 0;
				}
				// ��ų �ߵ��ð�
				if (boss_Info.SkillTimer <= 10)
					boss_Info.SkillTimer++;
				else
				{
					boss_Info.isSkill = true;
					for (int i = 0; i < 5; i++)
					{
						boss_Info.B_Bullet[0][i].isShot = true;
					}
				}
				// ��ų ����Ʈ
				if (boss_Info.isSkill)
				{
					if (boss_Info.SkillIndex <= 8)
						boss_Info.SkillIndex++;
					else
					{
						boss_Info.isSkill = false;
						boss_Info.SkillTimer = 0;
						boss_Info.SkillIndex = 0;
					}
				}
				// ��ų �ִϸ��̼�
				for (int i = 0; i < 5; i++)
				{
					if (boss_Info.B_Bullet[0][i].isShot)
					{
						if (boss_Info.B_Bullet[0][i].bulletIndex <= 1)
						{
							boss_Info.B_Bullet[0][i].bulletIndex++;
						}
						else
							boss_Info.B_Bullet[0][i].bulletIndex = 0;
					}
				}

				// ��� ���
				if (boss_Info.isDie)
				{
					if (boss_Info.DieIndex <= 7)
						boss_Info.DieIndex++;
					else
					{
						boss_Info.isDie = false;
						boss_Info.isLive = false;
						boss_Info.DieIndex = 0;
						phase += 1;
						Cur_Info.Level++;
					}
				}
			}
#pragma endregion
#pragma region 2�� ����
			// 2�� ����
			if (phase == 2)
			{
				if (boss_Info.isLive && phase == 2)
				{
					if (boss_Info.walkIndex <= 3)
						boss_Info.walkIndex++;
					else
						boss_Info.walkIndex = 0;
				}

				// ��ų �ߵ��ð�
				if (boss_Info.SkillTimer <= 10)
					boss_Info.SkillTimer++;
				else
				{
					boss_Info.isSkill = true;
				}

				// ��ų ����Ʈ
				if (boss_Info.isSkill)
				{
					if (boss_Info.SkillIndex <= 13)
						boss_Info.SkillIndex++;
					else
					{
						boss_Info.isSkill = false;
						boss_Info.SkillTimer = 0;
						boss_Info.SkillIndex = 0;
					}
				}

				// ��� ���
				if (boss_Info.isDie)
				{
					if (boss_Info.DieIndex <= 4)
						boss_Info.DieIndex++;
					else
					{
						boss_Info.isDie = false;
						boss_Info.isLive = false;
						boss_Info.DieIndex = 0;
						phase += 1;
						Cur_Info.Level++;
					}
				}
			}
#pragma endregion
#pragma region 3�� ����
			// 3�� ����
			if (phase == 3)
			{
				if (boss_Info.isLive && phase == 3)
				{
					if (boss_Info.walkIndex <= 4)
						boss_Info.walkIndex++;
					else
						boss_Info.walkIndex = 0;
				}

				// ��ų �ߵ��ð�
				if (boss_Info.SkillTimer <= 20)
					boss_Info.SkillTimer++;
				else
				{
					boss_Info.isSkill = true;
					for (int i = 0; i < 5; i++)
					{
						boss_Info.B_Bullet[0][i].isShot = true;
					}
				}

				// ��ų ����Ʈ
				if (boss_Info.isSkill)
				{
					if (boss_Info.SkillIndex <= 10)
						boss_Info.SkillIndex++;
					else
					{
						boss_Info.isSkill = false;
						boss_Info.SkillTimer = 0;
						boss_Info.SkillIndex = 0;
					}
				}

				// ��ų �ִϸ��̼�
				for (int i = 0; i < 5; i++)
				{
					if (boss_Info.B_Bullet[0][i].isShot)
					{
						if (boss_Info.B_Bullet[0][i].bulletIndex <= 2)
						{
							boss_Info.B_Bullet[0][i].bulletIndex++;
						}
						else
							boss_Info.B_Bullet[0][i].bulletIndex = 0;
					}
				}

				// ��� ���
				if (boss_Info.isDie)
				{
					if (boss_Info.DieIndex <= 9)
						boss_Info.DieIndex++;
					else
					{
						boss_Info.isDie = false;
						boss_Info.isLive = false;
						boss_Info.DieIndex = 0;
						phase += 1;
						Cur_Info.Level++;
					}
				}
			}
#pragma endregion
#pragma region 4�� ����
			// 4�� ����
			if (phase == 4)
			{
				if (boss_Info.isLive && phase == 4)
				{
					if (boss_Info.walkIndex <= 10)
						boss_Info.walkIndex++;
					else
						boss_Info.walkIndex = 0;
				}

				// ��ų �ߵ��ð�
				if (boss_Info.SkillTimer <= 20)
					boss_Info.SkillTimer++;
				else
				{
					boss_Info.isSkillC = false;
					boss_Info.isSkill = true;
				}

				// ��ų ����Ʈ
				if (boss_Info.isSkill)
				{
					if (boss_Info.SkillIndex <= 10)
						boss_Info.SkillIndex++;

					if (boss_Info.B_Bullet[0][0].bulletIndex <= 12)
					{
						boss_Info.B_Bullet[0][0].bulletIndex++;
					}
					else
					{
						boss_Info.isSkill = false;
						boss_Info.SkillTimer = 0;
						boss_Info.SkillIndex = 0;
						boss_Info.B_Bullet[0][0].bulletIndex = 0;
					}
				}

				// ��� ���
				if (boss_Info.isDie)
				{
					if (boss_Info.DieIndex <= 7)
						boss_Info.DieIndex++;
					else
					{
						boss_Info.isDie = false;
						boss_Info.isLive = false;
						boss_Info.DieIndex = 0;
						Player.P_CHANGE_ON = true;
					}
				}
			}
#pragma endregion
#pragma region 5�� ����
			// 5�� ����
			if (phase == 5)
			{
				if (boss_Info.isSpawn)
				{
					if (boss_Info.SpawnIndex <= 17)
						boss_Info.SpawnIndex++;
					else
					{
						boss_Info.SpawnIndex = 0;
						boss_Info.isSpawn = false;
						boss_Info.B_B = LoadBitmap(g_hInst, MAKEINTRESOURCE(IDB_BOSS_5));
					}
				}

				if (boss_Info.isLive && phase == 5 && !boss_Info.isSpawn)
				{
					if (boss_Info.walkIndex <= 6)
						boss_Info.walkIndex++;
					else
						boss_Info.walkIndex = 0;
				}

				// ��ų �ߵ��ð�
				if (!boss_Info.isSkill)
				{
					if (boss_Info.SkillTimer <= 20)
						boss_Info.SkillTimer++;
					else
					{
						boss_Info.isSkill = true;
						boss_Info.isSkillC = false;
						for (int i = 0; i < 5; i++)
						{
							boss_Info.B_Bullet[0][i].isShot = true;
						}
					}
				}

				// ��ų ����Ʈ
				if (boss_Info.isSkill && !boss_Info.isSpawn)
				{
					if (boss_Info.SkillIndex <= 9)
						boss_Info.SkillIndex++;

					else
					{
						boss_Info.isSkill = false;
						boss_Info.SkillTimer = 0;
						boss_Info.SkillIndex = 0;
					}
				}

				// ��� ���
				if (boss_Info.isDie && !boss_Info.isSpawn)
				{
					if (boss_Info.DieIndex <= 8)
						boss_Info.DieIndex++;
					else
					{
						boss_Info.isDie = false;
						boss_Info.isLive = false;
						boss_Info.DieIndex = 0;
						phase += 1;
						Cur_Info.Level++;
					}
				}
			}
#pragma endregion

			break;
		case 3:
			// 1�� ��ų
			if (Player.SKILL_1)
			{
				if (Player.SkillIndex <= 8)
				{
					Player.SkillIndex++;
				}
				else
				{
					Player.SkillIndex = 0;
					Player.SKILL_1 = false;
					Player.SkillOn = false;
					SetRect(&Player.P_SKILLRT, -200, -200, -200, -200);
				}
			}
			// 2�� ��ų
			if (Player.SKILL_2)
			{
				if (Player.SkillIndex <= 11)
				{
					Player.SkillIndex++;
				}
				else
				{
					Player.SkillIndex = 0;
					Player.SKILL_2 = false;
					Player.SkillOn = false;
					SetRect(&Player.P_SKILLRT, -200, -200, -200, -200);
				}
			}
			// 3�� ��ų
			if (Player.SKILL_3)
			{
				if (Player.SkillIndex <= 17)
				{
					Player.SkillIndex++;
				}
				else
				{
					Player.SkillIndex = 0;
					Player.SKILL_3 = false;
					Player.SkillOn = false;
					SetRect(&Player.P_SKILLRT, -200, -200, -200, -200);
				}
			}
			// 4�� ��ų
			if (Player.SKILL_4)
			{
				if (Player.SkillIndex <= 16)
				{
					Player.SkillIndex++;
				}
				else
				{
					Player.SkillIndex = 0;
					Player.SKILL_4 = false;
					Player.SkillOn = false;
					SetRect(&Player.P_SKILLRT, -200, -200, -200, -200);
				}
			}
			// 5�� ��ų
			if (Player.SKILL_5)
			{
				if (Player.SkillIndex <= 200)
				{
					Player.SkillIndex++;
					if (Player.SkillIndex >= 14)
					{
						if (Player.P_SKILL_5_INDEX <= 10)
						{
							Player.P_SKILL_5_INDEX++;
						}
						else
							Player.P_SKILL_5_INDEX = 0;
					}
				}
				else
				{
					Player.SkillIndex = 0;
					Player.SKILL_5 = false;
					Player.SkillOn = false;
					SetRect(&Player.P_SKILLRT, -200, -200, -200, -200);
				}
			}
			// 6�� ��ų
			if (Player.SKILL_6)
			{
				if (Player.SkillIndex <= 11)
				{
					Player.SkillIndex++;
				}
				else
				{
					Player.SkillIndex = 0;
					Player.SKILL_6 = false;
					Player.SkillOn = false;
					SetRect(&Player.P_SKILLRT, -200, -200, -200, -200);
				}
			}

			// ������ ����Ʈ
			if (Player.isLevelUp)
			{
				if (Player.LevelUpIndex <= 18)
				{
					Player.LevelUpIndex++;
				}
				else
				{
					Player.LevelUpIndex = 0;
					Player.isLevelUp = false;
				}
			}


			break;
		case 4:

			// �÷��̾� ��ȭ �ִϸ��̼�
			if (Player.P_CHANGE_ON)
			{
				if (Player.P_CHANGE_INDEX <= 5)
				{
					if (Player.P_CHANGE_ANI_INDEX <= 8)
					{
						Player.P_CHANGE_ANI_INDEX++;
					}
					else
					{
						Player.P_CHANGE = LoadBitmap(g_hInst, MAKEINTRESOURCE(IDB_PLAYER_CHANGE_1 + Player.P_CHANGE_INDEX));
						Player.P_CHANGE_ANI_INDEX = 0;
						Player.P_CHANGE_INDEX++;
					}
				}
				else
				{
					if (Player.P_CHANGE_ANI_INDEX <= 9)
					{
						Player.P_CHANGE_ANI_INDEX++;
					}
					else
					{
						Player.P_CHANGE_ANI_INDEX = 0;
						Player.P_CHANGE_INDEX++;
						Player.P_CHANGE_ON = false;
						Player.health = 100;
						phase += 1;
						Cur_Info.Level++;
						Player.P_B = LoadBitmap(g_hInst, MAKEINTRESOURCE(IDB_PLAYER_CHANGE_WALKING));
					}
				}
			}

			break;
		}
		return 0;
	case WM_COMMAND:
		switch (wParam)
		{
		case ID_REGISTER: // ȸ������ ���� �� 
			DialogBox(g_hInst, MAKEINTRESOURCE(IDD_REGISTER), hWndMain, RegisterProc); // RegisterProc ��ȭ���� ����
			break;
		case ID_LOGIN: // �α��� ��ư ���� ��
			DialogBox(g_hInst, MAKEINTRESOURCE(IDD_LOGIN), hWndMain, LoginProc); // LoginProc ��ȭ���� ����
			break;
		case ID_LOGOUT:
			phase = -1;
			Cur_Info.GameStart = false;
			// �α���, ȸ������ ��ư ��Ȱ��ȭ 
			EnableMenuItem(hMenu, ID_LOGIN, MF_ENABLED);
			EnableMenuItem(hMenu, ID_LOGOUT, MF_DISABLED);
			EnableMenuItem(hMenu, ID_REGISTER, MF_ENABLED);
			break;
		case ID_EXIT: // ���� ��ư ���� ��
			exit(0);
		}
		return 0;
	case WM_LBUTTONDOWN:
		Cur_Info.pt.x = LOWORD(lParam);
		Cur_Info.pt.y = HIWORD(lParam);
		return 0;
	case WM_PAINT:
		hdc = BeginPaint(hWnd, &ps);
		//if (hBit) DrawBitmap(hdc, 0, 0, hBit);
		if (hBackBit) DrawBitmap(hdc, 0, 0, hBackBit);
		//DrawBitmap(hdc, 0, 0, BackGround);
		EndPaint(hWnd, &ps);
		return 0;
	case WM_DESTROY: // ���� �������� ����.
		DBDisConnect();
		PostQuitMessage(0);
		return 0;
	}
	return(DefWindowProc(hWnd, iMessage, wParam, lParam));
}

// ȸ������ ��ȭ���� Proc
BOOL CALLBACK RegisterProc(HWND hDlig, UINT iMessage, WPARAM wParam, LPARAM lParam)
{
	TCHAR p_name[50], p_id[50], p_pw[50], p_email[50]; // TCHAR �� �迭(���ڿ�) ����
	int retval;

	switch (iMessage)
	{
	case WM_INITDIALOG:
		return TRUE;
	case WM_COMMAND:
		switch (LOWORD(wParam))
		{
		case IDOK: // OK ��������
			// ���ڿ��� �޾ƿ�.
			GetDlgItemText(hDlig, IDC_EDIT_REGISTER_NAME, p_name, sizeof(p_name));
			GetDlgItemText(hDlig, IDC_EDIT_REGISTER_ID, p_id, sizeof(p_id));
			GetDlgItemText(hDlig, IDC_EDIT_REGISTER_PW, p_pw, sizeof(p_pw));
			GetDlgItemText(hDlig, IDC_EDIT_REGISTER_EMAIL, p_email, sizeof(p_email));

			// ��� ��ĭ�� �ƴ϶��
			if (!(lstrlen(p_name) == 0 || lstrlen(p_id) == 0 || lstrlen(p_pw) == 0 || lstrlen(p_email) == 0))
			{
				wsprintf(str, TEXT("insert into LoginTbl values('%s','%s','%s','%s','%d')"), p_name, p_id, p_pw, p_email, 0); // ���ڿ� str�� ������
				SQLCloseCursor(hStmt); // Ŀ���� �ݰ� �������� ��� ���� ��

				retval = SQLExecDirect(hStmt, (SQLCHAR*)str, SQL_NTS);

				if (retval == SQL_ERROR) // �̹� ���̵� ������
				{
					MessageBox(hWndMain, TEXT("�̹� �ִ� ���̵� �Դϴ�!"), TEXT("���̵� �ߺ� ����"), MB_OK);
				}
			}
			else
			{
				MessageBox(hWndMain, TEXT("��� ������ �Է����ּ��� !"), TEXT("���̵� ���� ����"), MB_OK);
			}
			EndDialog(hDlig, IDOK);
			break;
		case IDCANCEL:
			EndDialog(hDlig, IDCANCEL);
			break;
		}
		return true;
	}
	return false;
}

// �α��� ��ȭ���� LoginProc
BOOL CALLBACK LoginProc(HWND hDlig, UINT iMessage, WPARAM wParam, LPARAM lParam)
{
	TCHAR p_id[50], p_pw[50];
	int retval;

	switch (iMessage)
	{
	case WM_INITDIALOG:
		return TRUE;
	case WM_COMMAND:
		switch (LOWORD(wParam))
		{
		case IDOK:
			// ���ڿ����� �޾ƿ�.
			GetDlgItemText(hDlig, IDC_EDIT_LOGIN_ID, p_id, sizeof(p_id));
			GetDlgItemText(hDlig, IDC_EDIT_LOGIN_PW, p_pw, sizeof(p_pw));

			SQLCloseCursor(hStmt); // Ŀ���� �����ϰ� �������� ����� ������.

			wsprintf(str, TEXT("select * from LoginTbl where P_ID='%s'"), p_id);

			retval = SQLExecDirect(hStmt, (SQLCHAR*)str, SQL_NTS);

			if (SQLFetch(hStmt) != SQL_NO_DATA)
			{
				if (!lstrcmp((TCHAR*)P_PASSWORD, p_pw)) // ��й�ȣ�� ���ٸ�
				{

					wsprintf(str, TEXT("%s ���� �α��� �ϼ̽��ϴ� !"), P_NAME);
					MessageBox(hDlig, TEXT(str), TEXT("�α��� ���"), MB_OK);
					// ���� �÷��̾� ID,EMAIL ������ ��������.
					lstrcpy(Cur_Info.Cur_ID, (TCHAR*)P_NAME);
					lstrcpy(Cur_Info.Cur_Email, (TCHAR*)P_EMAIL);
					// �α���, ȸ������ ��ư ��Ȱ��ȭ 
					EnableMenuItem(hMenu, ID_LOGIN, MF_DISABLED);
					EnableMenuItem(hMenu, ID_LOGOUT, MF_ENABLED);
					EnableMenuItem(hMenu, ID_REGISTER, MF_DISABLED);

					Cur_Info.GameStart = true;
					phase = 0;
					//Cur_Info.cur_score = 0;

					InvalidateRect(hWndMain, NULL, TRUE);

					EndDialog(hDlig, IDOK);
				}
				else // ��й�ȣ�� Ʋ���ٸ� 
				{
					MessageBox(hDlig, TEXT("�н����� ����!!"), TEXT("�α��� ���"), MB_OK);
				}
			}
			else // ���̵� ��ġ���� �ʴٸ�
			{
				MessageBox(hDlig, TEXT("���� ���̵��Դϴ�!!"), TEXT("�α��� ���"), MB_OK);
			}
			EndDialog(hDlig, IDOK);
			break;
		case IDCANCEL:
			EndDialog(hDlig, IDCANCEL);
			break;
		}
		return true;
	}
	return false;
}

// ���� �ʱ� �������ִ� �Լ�
void Init()
{
	// DB ����
	if (DBConnect() == FALSE) { // ������ ���̽��� ������ �� ���ٸ� 
		MessageBox(hWndMain, "������ ���̽��� ������ �� �����ϴ�", "����", MB_OK);
		return;
	}
	// DB ���ε����ֱ� ���� �Լ� ȣ��
	DBExecuteSQL();

	SetTimer(hWndMain, 1, 10, NULL);
	SetTimer(hWndMain, 2, 200, NULL); // ĳ���� �ȴ¸�� Ÿ�̸�
	SetTimer(hWndMain, 3, 50, NULL);
	SetTimer(hWndMain, 4, 100, NULL);

	Cur_Info.Clear = LoadBitmap(g_hInst, MAKEINTRESOURCE(IDB_CLEAR));

	Player.P_B = LoadBitmap(g_hInst, MAKEINTRESOURCE(IDB_PLAYER_WALKING));
	Player.P_SLOT = LoadBitmap(g_hInst, MAKEINTRESOURCE(IDB_QUICKSLOT));
	Player.P_LEVELUP = LoadBitmap(g_hInst, MAKEINTRESOURCE(IDB_PLAYER_LEVELUP));
	Player.P_CHANGE = LoadBitmap(g_hInst, MAKEINTRESOURCE(IDB_PLAYER_CHANGE_1));
	Player.P_HP = LoadBitmap(g_hInst, MAKEINTRESOURCE(IDB_PLAYER_HP));
	Player.P_HP_UI = LoadBitmap(g_hInst, MAKEINTRESOURCE(IDB_PLAYER_HP_UI));

	// ��ų
	Player.P_SKILL_1 = LoadBitmap(g_hInst, MAKEINTRESOURCE(IDB_PLAYER_SKILL_1));
	Player.P_SKILL_2 = LoadBitmap(g_hInst, MAKEINTRESOURCE(IDB_PLAYER_SKILL_2));
	Player.P_SKILL_3 = LoadBitmap(g_hInst, MAKEINTRESOURCE(IDB_PLAYER_SKILL_3));
	Player.P_SKILL_4 = LoadBitmap(g_hInst, MAKEINTRESOURCE(IDB_PLAYER_SKILL_4));
	Player.P_SKILL_5 = LoadBitmap(g_hInst, MAKEINTRESOURCE(IDB_PLAYER_SKILL_5_SUMMON));
	Player.P_SKILL_6 = LoadBitmap(g_hInst, MAKEINTRESOURCE(IDB_PLAYER_SKILL_6));

	// ��ų ������
	Player.P_SKILL_3_ICON = LoadBitmap(g_hInst, MAKEINTRESOURCE(IDB_PLAYER_SKILL_3_ICON));
	Player.P_SKILL_4_ICON = LoadBitmap(g_hInst, MAKEINTRESOURCE(IDB_PLAYER_SKILL_4_ICON));
	Player.P_SKILL_5_ICON = LoadBitmap(g_hInst, MAKEINTRESOURCE(IDB_PLAYER_SKILL_5_ICON));
	Player.P_SKILL_6_ICON = LoadBitmap(g_hInst, MAKEINTRESOURCE(IDB_PLAYER_SKILL_6_ICON));

	Cur_Info.inGameBg = LoadBitmap(g_hInst, MAKEINTRESOURCE(IDB_BACKGROUND));

	// �� ����
	for (int k = 0; k < 9; k++)
	{
		for (int i = 0; i < 5; i++)
		{
			enemy_Info[k][i].E_B = LoadBitmap(g_hInst, MAKEINTRESOURCE(IDB_ENEMY_1 + k));
		}
	}

	boss_Info.B_HPB = LoadBitmap(g_hInst, MAKEINTRESOURCE(IDB_BOSS_HP));

	// ������
	CloseHandle(CreateThread(NULL, 0, Thread1, NULL, 0, NULL));

	hEvent1 = CreateEvent(NULL, TRUE, FALSE, NULL);    // �̺�Ʈ ����

	// �α׾ƿ� ��ư ��Ȱ��ȭ
	EnableMenuItem(hMenu, ID_LOGOUT, MF_DISABLED);
}

void GetKey() // GetKeyState �޾ƿ� �Լ�
{
	if ((GetKeyState(VK_SPACE) & 0x8000))
	{
		g_bJumpkeyPressed = TRUE;
	}
	if (GetKeyState(VK_CONTROL) & 0x8000 && Player.SkillOn == false)
	{
		Player.SKILL_1 = true;
		Player.SkillOn = true;
	}
	if (GetKeyState(VK_SHIFT) & 0x8000 && Player.SkillOn == false)
	{
		Player.SKILL_2 = true;
		Player.SkillOn = true;
	}
	if (GetKeyState(VK_DELETE) & 0x8000 && Player.SkillOn == false && Player.SKILL_3_ENABLE)
	{
		Player.SKILL_3 = true;
		Player.SkillOn = true;
	}
	if (GetKeyState(VK_END) & 0x8000 && Player.SkillOn == false && Player.SKILL_4_ENABLE)
	{
		Player.SKILL_4 = true;
		Player.SkillOn = true;
	}
	if (GetKeyState(VK_INSERT) & 0x8000 && Player.SkillOn == false && Player.SKILL_5_ENABLE)
	{
		Player.SKILL_5 = true;
		Player.SkillOn = true;
	}
	if (GetKeyState(VK_HOME) & 0x8000 && Player.SkillOn == false && Player.SKILL_6_ENABLE)
	{
		Player.SKILL_6 = true;
		Player.SkillOn = true;
	}

	SetRect(&Player.P_RT, Player.px + 10, Player.py + 20, Player.px + 40, Player.py + 30); // �÷��̾� RECT ����.
}

void Jump()
{
	if (!g_bJumpkeyPressed) return;

	JumpHeight = (JumpTime * JumpTime - JumpPower * JumpTime) / 5.f;
	JumpTime += 0.4f;

	if (JumpTime > JumpPower)
	{
		JumpTime = 0;
		JumpHeight = 0;
		g_bJumpkeyPressed = FALSE;
	}
}

void Scroll(HDC hMemDC) // ����� ��ũ�� ���ֱ� ���� �Լ�
{
	HDC MemDC;
	HBITMAP OldBitmap;

	MemDC = CreateCompatibleDC(hdc);
	OldBitmap = (HBITMAP)SelectObject(MemDC, Cur_Info.inGameBg);

	if (x < 0)
	{
		BitBlt(hMemDC, 0, 0, -x, 775, MemDC, 1500 + x, 0, SRCCOPY);
		BitBlt(hMemDC, -x, 0, 1080 + x, 775, MemDC, 0, 0, SRCCOPY);
	}
	else
	{
		BitBlt(hMemDC, 0, 0, 1080, 775, MemDC, x, 0, SRCCOPY);
	}

	x += 10;

	if (x + 1080 >= 1500)
	{
		x -= 1500;
	}

	SelectObject(MemDC, OldBitmap);
	DeleteDC(MemDC);
}

void Stage(RECT rt)
{
	while (1)
	{
		// ���� ���۸��� ���� ����
		HDC hDC = GetDC(hWndMain);
		HDC hMemDC = CreateCompatibleDC(hDC);
		HBITMAP hOldBit = (HBITMAP)SelectObject(hMemDC, (HBITMAP)hBackBit);

		FillRect(hMemDC, &rt, GetSysColorBrush(COLOR_WINDOW));

		Scroll(hMemDC);

		Jump();

		Player.py = 570 + JumpHeight;

		if (Cur_Info.Level <= 8)
			AnimBlt(hMemDC, Player.px, Player.py, 114, 74, 114 * Player.WalkIndex, 0, Player.P_B, TrashColor);
		else
		{
			AnimBlt(hMemDC, Player.px, Player.py, 143, 84, 143 * Player.WalkIndex, 0, Player.P_B, TrashColor);
		}

		// ��ų��
		if (Player.SKILL_1)
		{
			if (Cur_Info.Level == 2)
			{
				AnimBlt(hMemDC, 120, 520, 421, 152, 421 * Player.SkillIndex, 0, Player.P_SKILL_1, TrashColor);
				SetRect(&Player.P_SKILLRT, 120, 520, 541, 672);
			}

			if (Cur_Info.Level >= 4)
			{
				AnimBlt(hMemDC, 120, 520, 380, 136, 380 * Player.SkillIndex, 0, Player.P_SKILL_1, TrashColor);
				SetRect(&Player.P_SKILLRT, 120, 520, 500, 656);
			}
		}
		if (Player.SKILL_2)
		{
			AnimBlt(hMemDC, -50, 520, 547, 221, 547 * Player.SkillIndex, 0, Player.P_SKILL_2, TrashColor);
			SetRect(&Player.P_SKILLRT, -50, 520, 497, 741);
		}
		if (Player.SKILL_3 && Player.SKILL_3_ENABLE)
		{
			AnimBlt(hMemDC, 60, 470, 422, 181, 422 * Player.SkillIndex, 0, Player.P_SKILL_3, TrashColor);
			SetRect(&Player.P_SKILLRT, 60, 470, 482, 651);
		}
		if (Player.SKILL_4 && Player.SKILL_4_ENABLE)
		{
			AnimBlt(hMemDC, 60, 355, 808, 292, 808 * Player.SkillIndex, 0, Player.P_SKILL_4, TrashColor);
			SetRect(&Player.P_SKILLRT, 60, 355, 868, 655);
		}
		if (Player.SKILL_5 && Player.SKILL_5_ENABLE)
		{
			if (Player.SkillIndex <= 14)
			{
				Player.P_SKILL_5 = LoadBitmap(g_hInst, MAKEINTRESOURCE(IDB_PLAYER_SKILL_5_SUMMON));
				AnimBlt(hMemDC, 40, 510, 621, 148, 621 * Player.SkillIndex, 0, Player.P_SKILL_5, TrashColor);
			}
			else
			{
				Player.P_SKILL_5 = LoadBitmap(g_hInst, MAKEINTRESOURCE(IDB_PLAYER_SKILL_5));
				AnimBlt(hMemDC, 40, 500, 702, 148, 702 * Player.P_SKILL_5_INDEX, 0, Player.P_SKILL_5, TrashColor);
			}
			SetRect(&Player.P_SKILLRT, 0, 0, 1000, 780);
		}
		if (Player.SKILL_6 && Player.SKILL_6_ENABLE)
		{
			AnimBlt(hMemDC, 240, 250, 692, 400, 692 * Player.SkillIndex, 0, Player.P_SKILL_6, TrashColor);
			SetRect(&Player.P_SKILLRT, 240, 250, 932, 650);
		}
		// ����
		DrawBitmap(hMemDC, 0, 0, Player.P_SLOT);

		// �÷��̾� ü��
		TransBlt(hMemDC, Player.px+10, 530 + JumpHeight, Player.P_HP_UI, TrashColor);
		AnimBlt(hMemDC, Player.px + 10, 540 + JumpHeight, ((float)Player.health / (float)Player.pashealth) * 107, 18, 0, 0, Player.P_HP, TrashColor);

		// ���Կ� ��ų�� Ȱ��ȭ
		if (Cur_Info.Level >= 4)
		{
			DrawBitmap(hMemDC, 44, 37, Player.P_SKILL_3_ICON);
			Player.SKILL_3_ENABLE = true;
			if (Cur_Info.Level >= 5)
			{
				DrawBitmap(hMemDC, 79, 37, Player.P_SKILL_4_ICON);
				Player.SKILL_4_ENABLE = true;
			}

			if (Cur_Info.Level >= 7)
			{
				DrawBitmap(hMemDC, 44, 2, Player.P_SKILL_5_ICON);
				Player.SKILL_5_ENABLE = true;
			}

			if (Cur_Info.Level >= 9)
			{
				DrawBitmap(hMemDC, 79, 2, Player.P_SKILL_6_ICON);
				Player.SKILL_6_ENABLE = true;
			}
		}

		if (Player.isLevelUp)
		{
			AnimBlt(hMemDC, 110, 200, 904, 488, 904 * Player.LevelUpIndex, 0, Player.P_LEVELUP, TrashColor);
		}

		if (Cur_Info.Level == 1)
		{
			for (int i = 0; i < 3; i++)
			{
				for (int k = 0; k < 5; k++)
				{
					enemy_Info[i][k].isLive = true;
					enemy_Info[i][k].px = 1050 + 200 * k + i * 1000;
				}
			}

			for (int i = 0; i < 1; i++)
			{
				boss_Info.B_Bullet[0][i].EB_BITMAP = LoadBitmap(g_hInst, MAKEINTRESOURCE(IDB_BOSS_1_SKILL_EFFECT));
				boss_Info.B_Bullet[0][i].EB_POS_x = 1500;
				boss_Info.B_Bullet[0][i].EB_POS_y = 580;
			}

			boss_Info.B_B = LoadBitmap(g_hInst, MAKEINTRESOURCE(IDB_BOSS_1_MOVE));
			boss_Info.isLive = true;
			boss_Info.px = 4050;
			boss_Info.pasHealth = 200000;
			boss_Info.health = 200000;

			Cur_Info.Level = 2;
			phase = 1;
		}

		if (phase == 1)
		{
			if (boss_Info.health >= 0)
				AnimBlt(hMemDC, 200, 0, ((float)boss_Info.health / (float)boss_Info.pasHealth) * 677, 16, 0, 0, boss_Info.B_HPB, TrashColor);

			if (boss_Info.px >= 400)
				boss_Info.px -= 10;

			for (int k = 0; k < 3; k++)
			{
				for (int i = 0; i < 5; i++)
				{
					if (enemy_Info[k][i].px >= 100)
						enemy_Info[k][i].px -= 1;

					// 1�� ����
					if (enemy_Info[0][i].isLive)
					{
						AnimBlt(hMemDC, enemy_Info[0][i].px, 610, 37, 26, 37 * enemy_Info[0][i].walkIndex, 0, enemy_Info[0][i].E_B, TrashColor);
						SetRect(&enemy_Info[0][i].E_RT, enemy_Info[0][i].px, 610, enemy_Info[0][i].px + 37, 636);
					}
					// 2�� ����
					if (enemy_Info[1][i].isLive)
					{
						AnimBlt(hMemDC, enemy_Info[1][i].px, 600, 41, 34, 41 * enemy_Info[1][i].walkIndex, 0, enemy_Info[1][i].E_B, TrashColor);
						SetRect(&enemy_Info[1][i].E_RT, enemy_Info[1][i].px, 600, enemy_Info[1][i].px + 41, 634);
					}
					// 3�� ����
					if (enemy_Info[2][i].isLive)
					{
						AnimBlt(hMemDC, enemy_Info[2][i].px, 600, 41, 34, 41 * enemy_Info[2][i].walkIndex, 0, enemy_Info[2][i].E_B, TrashColor);
						SetRect(&enemy_Info[2][i].E_RT, enemy_Info[2][i].px, 600, enemy_Info[2][i].px + 41, 634);
					}
				}
			}

			// 1�� ����
			if (boss_Info.isLive)
			{
				if (boss_Info.isHit && !boss_Info.isDie)
				{
					TransBlt(hMemDC, boss_Info.px, 540, boss_Info.B_B, TrashColor);
				}
				else if (!boss_Info.isHit && !boss_Info.isSkill && !boss_Info.isDie)
				{
					AnimBlt(hMemDC, boss_Info.px, 540, 106, 99, 106 * boss_Info.walkIndex, 0, boss_Info.B_B, TrashColor);
				}
				else if (boss_Info.isSkill && !boss_Info.isDie)
				{
					boss_Info.B_B = LoadBitmap(g_hInst, MAKEINTRESOURCE(IDB_BOSS_1_SKILL));
					AnimBlt(hMemDC, boss_Info.px - 20, 487, 147, 155, 147 * boss_Info.SkillIndex, 0, boss_Info.B_B, TrashColor);
				}
				else if (boss_Info.isDie)
				{
					AnimBlt(hMemDC, boss_Info.px - 5, 517, 125, 117, 125 * boss_Info.DieIndex, 0, boss_Info.B_B, TrashColor);
				}
				SetRect(&boss_Info.B_RT, boss_Info.px, 540, boss_Info.px + 106, 639);

				for (int i = 0; i < 1; i++)
				{
					if (boss_Info.B_Bullet[0][i].isShot)
					{
						if (boss_Info.B_Bullet[0][i].EB_POS_x >= 0)
						{
							boss_Info.B_Bullet[0][i].EB_POS_x -= 7;
							AnimBlt(hMemDC, boss_Info.B_Bullet[0][i].EB_POS_x, boss_Info.B_Bullet[0][i].EB_POS_y, 34, 35, 34 * boss_Info.B_Bullet[0][i].bulletIndex, 0, boss_Info.B_Bullet[0][i].EB_BITMAP, TrashColor);
							SetRect(&boss_Info.B_Bullet[0][i].bulletRect, boss_Info.B_Bullet[0][i].EB_POS_x + 5, boss_Info.B_Bullet[0][i].EB_POS_y
								, boss_Info.B_Bullet[0][i].EB_POS_x + 34, boss_Info.B_Bullet[0][i].EB_POS_y + 35);
						}
						else
						{
							SetRect(&boss_Info.B_Bullet[0][i].bulletRect, -200, -200
								, -200, -200);

							boss_Info.B_Bullet[0][i].EB_POS_x = 1500;
							boss_Info.B_Bullet[0][i].isShot = false;
						}
					}
				}
			}
		}

		if (Cur_Info.Level == 3)
		{
			Player.P_SKILL_1 = LoadBitmap(g_hInst, MAKEINTRESOURCE(IDB_PLAYER_SKILL_1_2));

			for (int i = 3; i < 6; i++)
			{
				for (int k = 0; k < 5; k++)
				{
					enemy_Info[i][k].isLive = true;
					enemy_Info[i][k].px = 1050 + 200 * k + (i - 3) * 1000;
				}
			}

			boss_Info.B_B = LoadBitmap(g_hInst, MAKEINTRESOURCE(IDB_BOSS_2_MOVE));
			boss_Info.B_SKILL_EFFECT = LoadBitmap(g_hInst, MAKEINTRESOURCE(IDB_BOSS_2_SKILL_EFFECT));
			boss_Info.isLive = true;
			boss_Info.isHit = false;
			boss_Info.isDie = false;
			boss_Info.isSkill = false;
			boss_Info.px = 4050;
			boss_Info.pasHealth = 300000;
			boss_Info.health = 300000;

			Cur_Info.Level = 4;
		}

		if (phase == 2)
		{
			if (boss_Info.health >= 0)
				AnimBlt(hMemDC, 200, 0, ((float)boss_Info.health / (float)boss_Info.pasHealth) * 677, 16, 0, 0, boss_Info.B_HPB, TrashColor);

			if (boss_Info.px >= 400)
				boss_Info.px -= 20;

			for (int k = 3; k < 6; k++)
			{
				for (int i = 0; i < 5; i++)
				{
					if (enemy_Info[k][i].px >= 100)
						enemy_Info[k][i].px -= 2;

					// 1�� ����
					if (enemy_Info[3][i].isLive)
					{
						AnimBlt(hMemDC, enemy_Info[3][i].px, 598, 37, 37, 37 * enemy_Info[3][i].walkIndex, 0, enemy_Info[3][i].E_B, TrashColor);
						SetRect(&enemy_Info[3][i].E_RT, enemy_Info[3][i].px, 610, enemy_Info[3][i].px + 37, 647);
					}
					// 2�� ����
					if (enemy_Info[4][i].isLive)
					{
						AnimBlt(hMemDC, enemy_Info[4][i].px, 587, 62, 54, 62 * enemy_Info[4][i].walkIndex, 0, enemy_Info[4][i].E_B, TrashColor);
						SetRect(&enemy_Info[4][i].E_RT, enemy_Info[4][i].px, 600, enemy_Info[4][i].px + 62, 654);
					}
					// 3�� ����
					if (enemy_Info[5][i].isLive)
					{
						AnimBlt(hMemDC, enemy_Info[5][i].px, 575, 64, 64, 64 * enemy_Info[5][i].walkIndex, 0, enemy_Info[5][i].E_B, TrashColor);
						SetRect(&enemy_Info[5][i].E_RT, enemy_Info[5][i].px, 575, enemy_Info[5][i].px + 64, 639);
					}
				}
			}

			// 2�� ����
			if (boss_Info.isLive)
			{
				if (boss_Info.isHit && !boss_Info.isDie)
				{
					TransBlt(hMemDC, boss_Info.px, 500, boss_Info.B_B, TrashColor);
				}
				else if (!boss_Info.isHit && !boss_Info.isSkill && !boss_Info.isDie)
				{
					AnimBlt(hMemDC, boss_Info.px, 510, 120, 125, 120 * boss_Info.walkIndex, 0, boss_Info.B_B, TrashColor);
				}
				else if (boss_Info.isSkill && !boss_Info.isDie)
				{
					boss_Info.B_B = LoadBitmap(g_hInst, MAKEINTRESOURCE(IDB_BOSS_2_SKILL));

					if (boss_Info.SkillIndex <= 7)
					{
						AnimBlt(hMemDC, boss_Info.px - 20, 407, 118, 125, 0, 0, boss_Info.B_B, TrashColor);
						AnimBlt(hMemDC, boss_Info.px - 20, 407, 254, 79, 254 * boss_Info.SkillIndex, 0, boss_Info.B_SKILL_EFFECT, TrashColor);
					}
					else
					{
						if (boss_Info.SkillIndex == 8)
						{
							if (Player.py >= 565)
								Player.health -= 1;
						}

						AnimBlt(hMemDC, boss_Info.px - 20, 510, 118, 125, 0, 0, boss_Info.B_B, TrashColor);
						AnimBlt(hMemDC, boss_Info.px - 70, 565, 254, 79, 254 * boss_Info.SkillIndex, 0, boss_Info.B_SKILL_EFFECT, TrashColor);
					}
				}
				else if (boss_Info.isDie)
				{
					AnimBlt(hMemDC, boss_Info.px - 5, 517, 191, 140, 191 * boss_Info.DieIndex, 0, boss_Info.B_B, TrashColor);
				}
				SetRect(&boss_Info.B_RT, boss_Info.px, 540, boss_Info.px + 106, 639);
			}
		}

		if (Cur_Info.Level == 5)
		{
			Player.P_SKILL_1 = LoadBitmap(g_hInst, MAKEINTRESOURCE(IDB_PLAYER_SKILL_1_2));

			for (int i = 6; i < 9; i++)
			{
				for (int k = 0; k < 5; k++)
				{
					enemy_Info[i][k].isLive = true;
					enemy_Info[i][k].px = 1050 + 200 * k + (i - 6) * 1000;
				}
			}

			for (int i = 0; i < 1; i++)
			{
				boss_Info.B_Bullet[0][i].EB_BITMAP = LoadBitmap(g_hInst, MAKEINTRESOURCE(IDB_BOSS_3_SKILL_EFFECT));
				boss_Info.B_Bullet[0][i].EB_POS_x = 1500;
				boss_Info.B_Bullet[0][i].EB_POS_y = 560;
			}

			boss_Info.B_B = LoadBitmap(g_hInst, MAKEINTRESOURCE(IDB_BOSS_3_MOVE));
			boss_Info.isLive = true;
			boss_Info.isHit = false;
			boss_Info.isDie = false;
			boss_Info.isSkill = false;
			boss_Info.px = 4050;
			boss_Info.pasHealth = 600000;
			boss_Info.health = 600000;

			Cur_Info.Level = 6;
		}

		if (phase == 3)
		{
			if (boss_Info.health >= 0)
				AnimBlt(hMemDC, 200, 0, ((float)boss_Info.health / (float)boss_Info.pasHealth) * 677, 16, 0, 0, boss_Info.B_HPB, TrashColor);

			if (boss_Info.px >= 700)
				boss_Info.px -= 10;

			for (int k = 6; k < 9; k++)
			{
				for (int i = 0; i < 5; i++)
				{
					if (enemy_Info[k][i].px >= 100)
						enemy_Info[k][i].px -= 10;

					// 1�� ����
					if (enemy_Info[6][i].isLive)
					{
						AnimBlt(hMemDC, enemy_Info[6][i].px, 550, 66, 92, 66 * enemy_Info[6][i].walkIndex, 0, enemy_Info[6][i].E_B, TrashColor);
						SetRect(&enemy_Info[6][i].E_RT, enemy_Info[6][i].px, 550, enemy_Info[6][i].px + 66, 642);
					}
					// 2�� ����
					if (enemy_Info[7][i].isLive)
					{
						AnimBlt(hMemDC, enemy_Info[7][i].px, 440, 172, 156, 172 * enemy_Info[7][i].walkIndex, 0, enemy_Info[7][i].E_B, TrashColor);
						SetRect(&enemy_Info[7][i].E_RT, enemy_Info[7][i].px, 440, enemy_Info[7][i].px + 172, 596);
					}
					// 3�� ����
					if (enemy_Info[8][i].isLive)
					{
						AnimBlt(hMemDC, enemy_Info[8][i].px, 520, 101, 118, 101 * enemy_Info[8][i].walkIndex, 0, enemy_Info[8][i].E_B, TrashColor);
						SetRect(&enemy_Info[8][i].E_RT, enemy_Info[8][i].px, 520, enemy_Info[8][i].px + 101, 638);
					}
				}
			}

			// 3�� ����
			if (boss_Info.isLive)
			{
				if (boss_Info.isHit && !boss_Info.isDie)
				{
					TransBlt(hMemDC, boss_Info.px, 310, boss_Info.B_B, TrashColor);
				}
				else if (!boss_Info.isHit && !boss_Info.isSkill && !boss_Info.isDie)
				{
					AnimBlt(hMemDC, boss_Info.px, 350, 207, 248, 207 * boss_Info.walkIndex, 0, boss_Info.B_B, TrashColor);
				}
				else if (boss_Info.isSkill && !boss_Info.isDie)
				{
					boss_Info.B_B = LoadBitmap(g_hInst, MAKEINTRESOURCE(IDB_BOSS_3_SKILL));

					AnimBlt(hMemDC, boss_Info.px - 70, 290, 303, 324, 303 * boss_Info.SkillIndex, 0, boss_Info.B_B, TrashColor);
				}
				else if (boss_Info.isDie)
				{
					AnimBlt(hMemDC, boss_Info.px - 5, 300, 210, 287, 210 * boss_Info.DieIndex, 0, boss_Info.B_B, TrashColor);
				}
				SetRect(&boss_Info.B_RT, boss_Info.px, 350, boss_Info.px + 207, 598);
			}

			for (int i = 0; i < 1; i++)
			{
				if (boss_Info.B_Bullet[0][i].isShot)
				{
					if (boss_Info.B_Bullet[0][i].EB_POS_x >= 0)
					{
						boss_Info.B_Bullet[0][i].EB_POS_x -= 7;
						AnimBlt(hMemDC, boss_Info.B_Bullet[0][i].EB_POS_x, boss_Info.B_Bullet[0][i].EB_POS_y, 150, 49, 150 * boss_Info.B_Bullet[0][i].bulletIndex, 0, boss_Info.B_Bullet[0][i].EB_BITMAP, TrashColor);
						SetRect(&boss_Info.B_Bullet[0][i].bulletRect, boss_Info.B_Bullet[0][i].EB_POS_x + 15, boss_Info.B_Bullet[0][i].EB_POS_y
							, boss_Info.B_Bullet[0][i].EB_POS_x + 150, boss_Info.B_Bullet[0][i].EB_POS_y + 49);
					}
					else
					{
						SetRect(&boss_Info.B_Bullet[0][i].bulletRect,-200, -200
							, -200, -200);
						boss_Info.B_Bullet[0][i].EB_POS_x = 1500;
						boss_Info.B_Bullet[0][i].isShot = false;
					}
				}
			}
		}

		if (Cur_Info.Level == 7)
		{
			boss_Info.B_Bullet[0][0].EB_BITMAP = LoadBitmap(g_hInst, MAKEINTRESOURCE(IDB_BOSS_4_SKILL_EFFECT));
			boss_Info.B_Bullet[0][0].EB_POS_x = 70;
			boss_Info.B_Bullet[0][0].EB_POS_y = 465;

			boss_Info.B_B = LoadBitmap(g_hInst, MAKEINTRESOURCE(IDB_BOSS_4));
			boss_Info.isLive = true;
			boss_Info.isHit = false;
			boss_Info.isDie = false;
			boss_Info.isSkill = false;
			boss_Info.px = 650;
			boss_Info.pasHealth = 1000000;
			boss_Info.health = 1000000;

			Cur_Info.Level = 8;
		}

		if (phase == 4)
		{
			if (boss_Info.health >= 0)
				AnimBlt(hMemDC, 200, 0, ((float)boss_Info.health / (float)boss_Info.pasHealth) * 677, 16, 0, 0, boss_Info.B_HPB, TrashColor);

			/*if (boss_Info.px >= 700)
				boss_Info.px -= 10;*/

				// 4�� ����
			if (boss_Info.isLive)
			{
				if (boss_Info.isHit && !boss_Info.isDie)
				{
					TransBlt(hMemDC, boss_Info.px, 310, boss_Info.B_B, TrashColor);
				}
				else if (!boss_Info.isHit && !boss_Info.isSkill && !boss_Info.isDie)
				{
					AnimBlt(hMemDC, boss_Info.px, 310, 419, 371, 419 * boss_Info.walkIndex, 0, boss_Info.B_B, TrashColor);
				}
				else if (boss_Info.isSkill && !boss_Info.isDie)
				{
					if (!boss_Info.isSkillC)
					{
						boss_Info.B_B = LoadBitmap(g_hInst, MAKEINTRESOURCE(IDB_BOSS_4_SKILL));
						boss_Info.isSkillC = true;
					}

					AnimBlt(hMemDC, boss_Info.px, 310, 416, 388, 416 * boss_Info.SkillIndex, 0, boss_Info.B_B, TrashColor);
					AnimBlt(hMemDC, boss_Info.B_Bullet[0][0].EB_POS_x, boss_Info.B_Bullet[0][0].EB_POS_y, 861, 202, 861 * boss_Info.B_Bullet[0][0].bulletIndex, 0, boss_Info.B_Bullet[0][0].EB_BITMAP, TrashColor);

					if (boss_Info.B_Bullet[0][0].bulletIndex >= 5)
						SetRect(&boss_Info.B_Bullet[0][0].bulletRect, 20, 500, 500, 700);
					else
						SetRect(&boss_Info.B_Bullet[0][0].bulletRect, -200, -200, -200, -200);
				}
				else if (boss_Info.isDie)
				{
					AnimBlt(hMemDC, boss_Info.px, 310, 467, 396, 467 * boss_Info.DieIndex, 0, boss_Info.B_B, TrashColor);
				}
				SetRect(&boss_Info.B_RT, boss_Info.px, 350, boss_Info.px + 207, 598);
			}
		}

		if (Cur_Info.Level == 9)
		{
			boss_Info.B_Bullet[0][0].EB_BITMAP = LoadBitmap(g_hInst, MAKEINTRESOURCE(IDB_BOSS_5_SKILL_EFEECT));
			boss_Info.B_Bullet[0][0].EB_POS_x = 70;
			boss_Info.B_Bullet[0][0].EB_POS_y = 525;

			boss_Info.B_B = LoadBitmap(g_hInst, MAKEINTRESOURCE(IDB_BOSS_5_SUMMON));
			boss_Info.isLive = true;
			boss_Info.isHit = false;
			boss_Info.isDie = false;
			boss_Info.isSkill = false;
			boss_Info.isSpawn = true;
			boss_Info.px = 650;
			boss_Info.pasHealth = 2000000;
			boss_Info.health = 2000000;

			Cur_Info.Level = 10;
		}

		if (phase == 5)
		{
			if (boss_Info.health >= 0)
				AnimBlt(hMemDC, 200, 0, ((float)boss_Info.health / (float)boss_Info.pasHealth) * 677, 16, 0, 0, boss_Info.B_HPB, TrashColor);

			/*if (boss_Info.px >= 700)
				boss_Info.px -= 10;*/

				// 4�� ����
			if (boss_Info.isLive)
			{
				if (boss_Info.isSpawn)
				{
					AnimBlt(hMemDC, boss_Info.px-40, 270, 697, 348, 697 * boss_Info.SpawnIndex, 0, boss_Info.B_B, TrashColor);
				}
				if (boss_Info.isHit && !boss_Info.isDie)
				{
					TransBlt(hMemDC, boss_Info.px, 350, boss_Info.B_B, TrashColor);
				}
				else if (!boss_Info.isHit && !boss_Info.isSkill && !boss_Info.isDie && !boss_Info.isSpawn)
				{
					AnimBlt(hMemDC, boss_Info.px, 350, 385, 301, 385 * boss_Info.walkIndex, 0, boss_Info.B_B, TrashColor);
				}
				else if (boss_Info.isSkill && !boss_Info.isDie)
				{
					if (!boss_Info.isSkillC)
					{
						boss_Info.B_B = LoadBitmap(g_hInst, MAKEINTRESOURCE(IDB_BOSS_5_SKILL));
						boss_Info.isSkillC = true;
					}

					AnimBlt(hMemDC, boss_Info.px, 350, 385, 299, 385 * boss_Info.SkillIndex, 0, boss_Info.B_B, TrashColor);
				}
				else if (boss_Info.isDie)
				{
					AnimBlt(hMemDC, boss_Info.px, 350, 385, 300, 385 * boss_Info.DieIndex, 0, boss_Info.B_B, TrashColor);
				}

				SetRect(&boss_Info.B_RT, boss_Info.px, 350, boss_Info.px + 207, 598);

				for (int i = 0; i < 3; i++)
				{
					if (boss_Info.B_Bullet[0][i].isShot)
					{
						if (boss_Info.B_Bullet[0][i].EB_POS_x >= 0)
						{
							boss_Info.B_Bullet[0][i].EB_POS_x -= 7;
							AnimBlt(hMemDC, boss_Info.B_Bullet[0][i].EB_POS_x, boss_Info.B_Bullet[0][i].EB_POS_y, 165, 111, 165 * boss_Info.B_Bullet[0][i].bulletIndex, 0, boss_Info.B_Bullet[0][i].EB_BITMAP, TrashColor);
							SetRect(&boss_Info.B_Bullet[0][i].bulletRect, boss_Info.B_Bullet[0][i].EB_POS_x + 15, boss_Info.B_Bullet[0][i].EB_POS_y
								, boss_Info.B_Bullet[0][i].EB_POS_x + 150, boss_Info.B_Bullet[0][i].EB_POS_y + 111);
						}
						else
						{
							boss_Info.B_Bullet[0][i].EB_POS_x = 1500;
							boss_Info.B_Bullet[0][i].isShot = false;
						}
					}
				}
			}
		}

		if (phase == 6)
		{
			TransBlt(hMemDC, 400, 200, Cur_Info.Clear, TrashColor);
			Player.px = 400;
		}

		if (Player.P_CHANGE_ON)
		{
			AnimBlt(hMemDC, 200, 200, 684, 384, 684 * Player.P_CHANGE_ANI_INDEX, 0, Player.P_CHANGE, TrashColor);
		}

		SelectObject(hMemDC, hOldBit);
		DeleteDC(hMemDC);
		ReleaseDC(hWndMain, hDC);


		InvalidateRect(hWndMain, NULL, FALSE);

		Collision();

		Sleep(10);
	}
}

void Collision()
{
	RECT prt;

	// �÷��̾� ��ų�� �Ϲ� ���� �ǰ� ����
	for (int i = 0; i < 9; i++)
	{
		for (int k = 0; k < 10; k++)
		{
			if (IntersectRect(&prt, &Player.P_SKILLRT, &enemy_Info[i][k].E_RT) && Player.SkillOn)
			{
				enemy_Info[i][k].isLive = false;
				SetRect(&enemy_Info[i][k].E_RT, -200, -200, -200, -200);
			}
			// �÷��̾�� ���� �浹
			if (IntersectRect(&prt, &Player.P_RT, &enemy_Info[i][k].E_RT))
			{
				Player.health -= 5;
				enemy_Info[i][k].isLive = false;
				SetRect(&enemy_Info[i][k].E_RT, -200, -200, -200, -200);
			}
		}
	}

	// ���� �Ѿ˰� �ǰ� ����.
	for (int i = 0; i < 1; i++)
	{
		if (IntersectRect(&prt, &Player.P_RT, &boss_Info.B_Bullet[0][i].bulletRect))
		{
			if (phase != 4)
				Player.health -= 10;
			if (phase == 4)
				Player.health -= 1;

			SetRect(&boss_Info.B_Bullet[0][i].bulletRect, -200, -200, -200, -200);
			boss_Info.B_Bullet[0][i].EB_POS_x = -200;
			boss_Info.B_Bullet[0][i].isShot = false;
		}
	}

	// �÷��̾� ��ų�� ���� �ǰ� ����
	if (IntersectRect(&prt, &Player.P_SKILLRT, &boss_Info.B_RT) && Player.SkillOn && !boss_Info.isSkill)
	{
		// 1 ������ ����
		if (phase == 1 && !boss_Info.isDie)
		{
			boss_Info.B_B = LoadBitmap(g_hInst, MAKEINTRESOURCE(IDB_BOSS_1_HIT));
			boss_Info.isHit = true;
			boss_Info.health -= 500;
			if (boss_Info.health <= 0)
			{
				boss_Info.B_B = LoadBitmap(g_hInst, MAKEINTRESOURCE(IDB_BOSS_1_DIE));
				boss_Info.isDie = true;
				Player.isLevelUp = true;
			}
		}

		// 2 ������ ����
		if (phase == 2 && !boss_Info.isDie)
		{
			boss_Info.B_B = LoadBitmap(g_hInst, MAKEINTRESOURCE(IDB_BOSS_2_HIT));
			boss_Info.isHit = true;
			boss_Info.health -= 500;
			if (boss_Info.health <= 0)
			{
				boss_Info.B_B = LoadBitmap(g_hInst, MAKEINTRESOURCE(IDB_BOSS_2_DIE));
				boss_Info.isDie = true;
				Player.isLevelUp = true;
			}
		}

		// 3 ������ ����
		if (phase == 3 && !boss_Info.isDie)
		{
			boss_Info.B_B = LoadBitmap(g_hInst, MAKEINTRESOURCE(IDB_BOSS_3_HIT));
			boss_Info.isHit = true;
			boss_Info.health -= 500;
			if (boss_Info.health <= 0)
			{
				boss_Info.B_B = LoadBitmap(g_hInst, MAKEINTRESOURCE(IDB_BOSS_3_DIE));
				boss_Info.isDie = true;
				Player.isLevelUp = true;
			}
		}

		// 4 ������ ����
		if (phase == 4 && !boss_Info.isDie)
		{
			boss_Info.B_B = LoadBitmap(g_hInst, MAKEINTRESOURCE(IDB_BOSS_4_HIT));
			boss_Info.isHit = true;
			boss_Info.health -= 500;
			if (boss_Info.health <= 0)
			{
				boss_Info.B_B = LoadBitmap(g_hInst, MAKEINTRESOURCE(IDB_BOSS_4_DIE));
				boss_Info.isDie = true;
				Player.isLevelUp = true;
			}
		}

		// 5 ������ ����
		if (phase == 5 && !boss_Info.isDie && !boss_Info.isSpawn)
		{
			boss_Info.B_B = LoadBitmap(g_hInst, MAKEINTRESOURCE(IDB_BOSS_5_HIT));
			boss_Info.isHit = true;
			boss_Info.health -= 500;
			if (boss_Info.health <= 0)
			{
				boss_Info.B_B = LoadBitmap(g_hInst, MAKEINTRESOURCE(IDB_BOSS_5_DIE));
				boss_Info.isDie = true;
				Player.isLevelUp = true;
			}
		}
		//SetRect(&enemy_Info[i][k].E_RT, -200, -200, -200, -200);
	}
	else
	{
		// 1 ������ ����
		if (phase == 1 && !boss_Info.isDie)
		{
			boss_Info.B_B = LoadBitmap(g_hInst, MAKEINTRESOURCE(IDB_BOSS_1_MOVE));
			boss_Info.isHit = false;
		}
		// 2 ������ ����
		if (phase == 2 && !boss_Info.isDie)
		{
			boss_Info.B_B = LoadBitmap(g_hInst, MAKEINTRESOURCE(IDB_BOSS_2_MOVE));
			boss_Info.isHit = false;
		}
		// 3 ������ ����
		if (phase == 3 && !boss_Info.isDie)
		{
			boss_Info.B_B = LoadBitmap(g_hInst, MAKEINTRESOURCE(IDB_BOSS_3_MOVE));
			boss_Info.isHit = false;
		}
		// 4 ������ ����
		if (phase == 4 && !boss_Info.isDie && !boss_Info.isSkill)
		{
			boss_Info.B_B = LoadBitmap(g_hInst, MAKEINTRESOURCE(IDB_BOSS_4));
			boss_Info.isHit = false;
		}
		// 5 ������ ����
		if (phase == 5 && !boss_Info.isDie && !boss_Info.isSpawn && !boss_Info.isSkill)
		{
			boss_Info.B_B = LoadBitmap(g_hInst, MAKEINTRESOURCE(IDB_BOSS_5));
			boss_Info.isHit = false;
		}
	}
}