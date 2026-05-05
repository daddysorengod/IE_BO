# WeatherForecastController API

Base route: `WeatherForecast`

## Ghi chu chung

- Day la controller sample mac dinh cua project.
- Endpoint khong yeu cau JWT token.
- Response van dung `ResponseBase<T>`.
- Khong thuoc workflow nghiep vu Export Order.

## Get weather forecast

- **Method**: `GET`
- **URL**: `WeatherForecast`
- **Auth**: khong can token

### Success (200)

```json
{
  "code": 0,
  "message": "Success",
  "data": [
    {
      "date": "2026-04-23",
      "temperatureC": 30,
      "temperatureF": 85,
      "summary": "Warm"
    }
  ]
}
```

### Luu y

- API tra ve 5 ban ghi ngau nhien.
- Nen xoa controller nay neu khong con dung cho demo/smoke test.
