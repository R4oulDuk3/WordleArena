{{- define "common.name" -}}
{{- if and (.Values.global) ((.Values.global).fullNameOverride) -}}
{{- tpl .Values.global.fullNameOverride . -}}
{{- else }}
{{- .Release.Name -}}-{{- .Chart.Name -}}
{{- end -}}
{{- end -}}

{{- define "common.environment" -}}
{{- if and (.Values.global) ((.Values.global).environment) -}}
{{- tpl .Values.global.environment . -}}
{{- else }}
{{- .Release.Name -}}
{{- end -}}
{{- end -}}

{{- define "common.service" -}}
{{- if $.serviceNameOverride -}}
{{- $.serviceNameOverride -}}
{{- else if and (.Values.global) ((.Values.global).service) -}}
{{- tpl .Values.global.service . -}}
{{- else }}
{{- .Chart.Name -}}
{{- end -}}
{{- end -}}

{{/*
App labels
*/}}
{{- define "common.labelsApp" -}}
{{ include "common.selectorLabelsApp" . }}
app.kubernetes.io/component: app
{{ include "common.labelsCommon" . }}
{{- end }}

{{/*
App Selector labels
*/}}
{{- define "common.selectorLabelsApp" -}}
# We need common.name as selector for canary release use-case where Chart.Name and environment are the same
# and only Release.Name and/or fullNameOverride are different.
name: {{ include "common.name" . }}
service: {{ include "common.service" . }}
environment: {{ include "common.environment" . }}
component: app
{{- end }}

{{/*
Common K8s/helm labels
*/}}
{{- define "common.labelsCommon" -}}
helm.sh/chart: {{ .Chart.Name }}-{{ .Chart.Version | replace "+" "_" }}
app.kubernetes.io/name: {{ .Chart.Name }}
app.kubernetes.io/instance: {{ .Release.Name }}
{{- if .Values.image }}
app.kubernetes.io/version: {{ .Values.image.tag | quote }}
{{- end }}
app.kubernetes.io/managed-by: {{ .Release.Service }}
app.kubernetes.io/part-of: {{ .Chart.Name }}
{{- end }}