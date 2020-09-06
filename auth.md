# /auth

{% api-method method="post" host="{SERVER\_IP}:{PORT}" path="/auth/login" %}
{% api-method-summary %}
로그인
{% endapi-method-summary %}

{% api-method-description %}
로그인 요청 API
{% endapi-method-description %}

{% api-method-spec %}
{% api-method-request %}
{% api-method-body-parameters %}
{% api-method-parameter name="id" type="string" required=true %}
사용자 ID
{% endapi-method-parameter %}

{% api-method-parameter name="pw" type="string" required=true %}
사용자 PW
{% endapi-method-parameter %}
{% endapi-method-body-parameters %}
{% endapi-method-request %}

{% api-method-response %}
{% api-method-response-example httpCode=200 %}
{% api-method-response-example-description %}
로그인 성
{% endapi-method-response-example-description %}

```
{
    "Login": {
        "data": {
            "email": "kkh03kkh@naver.com",
            "id": "KimKyungHoon",
            "name": "김경훈",
            "token": "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6Iuq5gOqyve2biCIsImVtYWlsIjoia2toMDNra2hAbmF2ZXIuY29tIiwibmJmIjoxNTk5MzkxMDA0LCJleHAiOjE2MDAwNDM4MDQsImlhdCI6MTU5OTM5MTAwNH0.xyQqmhfzLULFoCHbi1TiD6KGo7yozD_FgRg82TQoHgcBWHTO-jBIlefu6TgGWr90UEUQejksmwm-6juDbvKZOA"
        },
        "message": "성공적으로 처리되었습니다.",
        "status": 200
    }
}
```
{% endapi-method-response-example %}

{% api-method-response-example httpCode=400 %}
{% api-method-response-example-description %}
아이디 or 비밀번호 입력 안됨
{% endapi-method-response-example-description %}

```
{
    "Login": {
        "data": null,
        "message": "검증 오류.",
        "status": 400
    }
}
```
{% endapi-method-response-example %}

{% api-method-response-example httpCode=401 %}
{% api-method-response-example-description %}
아이디 또는 비밀번호가 일치하지 않음 또는 가입이 승인되지 않음
{% endapi-method-response-example-description %}

```
{
    "Login": {
        "data": null,
        "message": "아이디 또는 비밀번호를 확인하십시오.",
        "status": 401
    }
}
```
{% endapi-method-response-example %}

{% api-method-response-example httpCode=500 %}
{% api-method-response-example-description %}
서버오류
{% endapi-method-response-example-description %}

```
{
    "Login": {
        "data": null,
        "message": "서 오류.",
        "status": 500
    }
}
```
{% endapi-method-response-example %}
{% endapi-method-response %}
{% endapi-method-spec %}
{% endapi-method %}

{% api-method method="post" host="{SERVER\_IP}:{PORT}" path="/auth/login" %}
{% api-method-summary %}

{% endapi-method-summary %}

{% api-method-description %}

{% endapi-method-description %}

{% api-method-spec %}
{% api-method-request %}
{% api-method-path-parameters %}
{% api-method-parameter name="" type="string" required=false %}

{% endapi-method-parameter %}
{% endapi-method-path-parameters %}
{% endapi-method-request %}

{% api-method-response %}
{% api-method-response-example httpCode=400 %}
{% api-method-response-example-description %}
아이디 or 비밀번호 입력 안
{% endapi-method-response-example-description %}

```

```
{% endapi-method-response-example %}
{% endapi-method-response %}
{% endapi-method-spec %}
{% endapi-method %}



