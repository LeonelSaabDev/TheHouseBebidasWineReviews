export interface AdminLoginRequestDto {
  username: string
  password: string
}

export interface AdminLoginResponseDto {
  accessToken: string
  expiresAtUtc: string
  tokenType: string
}
