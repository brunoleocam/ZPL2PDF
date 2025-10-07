# 🔧 Pull Request Merge Troubleshooting

## ⚠️ **"Merging is blocked" - Como Resolver**

---

## 🔍 **DIAGNÓSTICO**

No PR, procure pela mensagem específica abaixo do botão "Merge pull request":

### **Tipo 1: ⏳ "Required status check(s) are expected"**

**Aparece:**
```
⏳ Some checks haven't completed yet
   ⏳ ci / test (In progress...)
```

**Causa:** GitHub Actions ainda está executando

**Solução:** Aguardar 2-5 minutos até os checks completarem

**Verificar em:** https://github.com/brunoleocam/ZPL2PDF/actions

---

### **Tipo 2: ❌ "Required status check(s) failed"**

**Aparece:**
```
❌ Some checks were not successful
   ❌ ci / test (Failed)
```

**Causa:** Testes falharam ou build deu erro

**Solução:**
1. Clicar no check que falhou
2. Ver o log de erro
3. Corrigir o problema
4. Fazer novo commit

---

### **Tipo 3: 🔒 "Review required"**

**Aparece:**
```
🔒 At least 1 approving review is required
```

**Causa:** Branch `main` tem proteção que requer aprovação

**Solução (você é admin):**
1. Aprovar seu próprio PR (botão "Review changes")
2. Ou desabilitar branch protection temporariamente:
   - Settings → Branches → Edit rule
   - Desmarcar "Require approvals"

---

### **Tipo 4: ⚔️ "This branch has conflicts"**

**Aparece:**
```
⚔️ This branch has conflicts that must be resolved
```

**Causa:** Alguém alterou `main` enquanto você trabalhava

**Solução:**
```bash
# Atualizar sua branch
git checkout release/v2.0.0
git fetch origin
git merge origin/main
# Resolver conflitos manualmente
git add .
git commit -m "fix: resolve merge conflicts"
git push origin release/v2.0.0
```

---

### **Tipo 5: 🔐 "Branch protection rule violations"**

**Aparece:**
```
🔐 Required status checks must pass before merging
```

**Causa:** Configuração de proteção de branch

**Solução (Admin Override):**
1. No PR, procurar por checkbox:
   - ☐ "Merge without waiting for requirements to be met"
2. Marcar o checkbox
3. Fazer merge

---

## 🛠️ **SOLUÇÃO RÁPIDA: Admin Override**

Se você é administrador do repositório:

### **No PR, procure por:**
```
☐ Use your admin privileges to merge this pull request.
```

**OU:**

```
☐ Merge without waiting for requirements to be met
   (bypass branch protections)
```

**Marque a checkbox** e o botão de merge ficará disponível.

---

## 🔓 **DESABILITAR BRANCH PROTECTION (Temporário)**

Se você quer desabilitar as proteções:

### **Passo a Passo:**

1. **Ir para Settings:**
   https://github.com/brunoleocam/ZPL2PDF/settings/branches

2. **Encontrar regra da branch `main`**

3. **Opções:**
   - **Opção A:** Deletar a regra completamente (menos seguro)
   - **Opção B:** Editar e desmarcar:
     - ☐ Require status checks to pass
     - ☐ Require approvals

4. **Fazer merge do PR**

5. **Reabilitar proteções** (recomendado)

---

## ⚡ **SOLUÇÃO MAIS RÁPIDA**

Se o problema for **GitHub Actions rodando**:

1. Ir para: https://github.com/brunoleocam/ZPL2PDF/actions
2. Ver workflows em andamento
3. **Aguardar** conclusão (geralmente 2-5 min)
4. Voltar ao PR e fazer merge

**OU:**

1. No PR, marcar: ☐ "Merge without waiting..."
2. Fazer merge imediatamente

---

## 📋 **VERIFICAR STATUS ATUAL**

Execute estes comandos para diagnosticar:

```bash
# Ver se há Actions rodando
gh run list --limit 5

# Ver status do PR
gh pr view 5 --json statusCheckRollup

# Ver branch protection
gh api repos/brunoleocam/ZPL2PDF/branches/main/protection
```

---

## 🎯 **RECOMENDAÇÃO**

**Para v2.0.0 (primeira release importante):**

1. ✅ **SE houver checks rodando:** Aguardar completar
2. ✅ **SE checks falharam:** Corrigir e fazer novo commit
3. ⚠️ **SE for só proteção de branch:** Usar admin override (checkbox)
4. ❌ **NÃO desabilitar proteções** (manter segurança)

---

## 📞 **ME DIGA:**

**Qual mensagem EXATA aparece no PR abaixo do botão "Merge pull request"?**

Exemplos:
- "⏳ Some checks haven't completed yet"
- "❌ Some checks were not successful"
- "🔒 At least 1 approving review is required"
- "⚔️ This branch has conflicts"
- Outra mensagem?

**Tire um print ou copie a mensagem para eu te ajudar especificamente!** 🔍
