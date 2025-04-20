# 📲 FilaDigital - Sistema de Gestão de Filas para o SUS

O **FilaDigital** é um sistema web que tem como objetivo melhorar a experiência de pacientes no SUS e otimizar o trabalho de profissionais de saúde. Ele permite que os pacientes acompanhem sua posição na fila de atendimento em tempo real diretamente do celular, promovendo maior autonomia, conforto e organização no atendimento.

---

## 🧩 Resumo Executivo

O projeto FilaDigital busca trazer mais dignidade e previsibilidade para quem depende do SUS. Através de uma API moderna e uma interface conectada, os pacientes podem visualizar seu lugar na fila de atendimento, reduzindo ansiedade e otimizando o fluxo nas unidades de saúde. O impacto esperado é um ambiente mais humano, organizado e seguro.

---

## ❗ Problema Identificado

Hoje, pacientes do SUS aguardam em filas sem clareza sobre sua posição ou tempo de espera. Muitos evitam sair da sala de espera com medo de perder a chamada, o que compromete seu bem-estar. As soluções atuais ainda são fixas e pouco integradas à rotina do paciente.

---

## 💡 Descrição da Solução

O FilaDigital permite:
- Geração de senha comum ou prioritária.
- Consulta da posição do paciente na fila.
- Atualização da etapa (guichê, triagem, médico etc).
- Visualização da fila atual (organizada por prioridade).
- Gerenciamento via API RESTful.

Pacientes recebem um código e podem acompanhar sua fila de forma digital, inclusive por QR Code.

---

## 🔧 Detalhes Técnicos

- **Linguagem:** C# (.NET 9)
- **Testes:** xUnit

### Estrutura
```
DigitalQueue
├── Domain
├── Application
├── Controllers
├── Program.cs
```

---

## 🧪 Endpoints da API

### Criar Paciente
```http
POST /api/v1/patient?priority={true|false}
```
**Retorna:** código do paciente (ex: P0001 ou C0001)

---

### Consultar Status do Paciente
```http
GET /api/v1/{code}
```
**Retorna:** nome da fila, pessoas à frente, código atual sendo atendido.

---

### Mover Paciente de Fila
```http
PUT /api/v1/{code}/{queueName}
```
**Body:** vazio  
**queueName:** Reception | Screening | MedicalCare | Exam | Medication

---

### Remover Paciente
```http
DELETE /api/v1/{code}
```

---

### Listar Fila Atual
```http
GET /api/v1/queue/{queueType}
```
**Retorna:** lista ordenada com todos os pacientes da fila (prioritários antes dos comuns).

---

## 🔁 Prioridade e Organização

A priorização funciona da seguinte forma:
- Senhas começam com `P` (prioritários) ou `C` (comuns).
- Ao entrar na fila, o paciente é posicionado:
  - Após o último prioritário (se for prioritário)
  - No final da fila (se for comum)

A fila respeita a ordem de chegada dentro da sua prioridade.

---

## 📈 Próximos Passos

- Conexão com sistemas existentes via API.
- Testes-piloto em unidades básicas de saúde (UBSs).
- Interface Web/Mobile para pacientes e profissionais.

---

## 📚 Aprendizados

- Experiência prática com APIs RESTful.
- Importância de soluções simples com grande impacto social.
- Valor da empatia no desenvolvimento de tecnologia pública.

---

> “Inovar no SUS não é só sobre tecnologia — é sobre empatia, acesso e impacto real na vida das pessoas.”