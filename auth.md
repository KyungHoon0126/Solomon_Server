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
{% endapi-method-response %}
{% endapi-method-spec %}
{% endapi-method %}



